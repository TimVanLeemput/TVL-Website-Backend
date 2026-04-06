# ---------- STAGE 1: "base" ----------
# Start from Microsoft's lightweight ASP.NET runtime image.
# This image can RUN .NET apps but can't BUILD them (no SDK).
# "AS base" names this stage so we can reference it later.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# ---------- STAGE 2: "build" ----------
# Start fresh from the full .NET SDK image.
# This image is much larger (~900MB vs ~200MB) because it includes
# the compiler, NuGet, dotnet CLI — everything needed to BUILD.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy ONLY the .csproj files first and restore.
# Docker caches each step. If .csproj hasn't changed, Docker skips
# the restore on subsequent builds — saving minutes.
COPY ["src/VotingPoll.API/VotingPoll.API.csproj", "src/VotingPoll.API/"]
COPY ["src/VotingPoll.Core/VotingPoll.Core.csproj", "src/VotingPoll.Core/"]
COPY ["src/VotingPoll.Infrastructure/VotingPoll.Infrastructure.csproj", "src/VotingPoll.Infrastructure/"]
RUN dotnet restore "src/VotingPoll.API/VotingPoll.API.csproj"

# Now copy everything else and publish a Release build.
COPY . .
RUN dotnet publish "src/VotingPoll.API/VotingPoll.API.csproj" -c Release -o /app/publish

# ---------- STAGE 3: "final" ----------
# Go back to the small runtime-only image (stage 1).
# Copy ONLY the published output from stage 2.
# The SDK, source code, and intermediate build files are discarded.
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "VotingPoll.API.dll"]