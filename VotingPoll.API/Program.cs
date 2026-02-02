//  The order is strict:
//1. Register services     (builder.Services.Add...) -> What the app will need and use
//2. Build the app         (builder.Build()) -> Builds the app
// -----------------------------------------------------------------------------------------------------------------
//3. Configure pipeline    (app.Use..., app.Map...)
//4. Run                   (app.Run())


// --------------------------------------------------------APP CONTAINER / SETUP--------------------------------------
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

WebApplication app = builder.Build();
// ----------------------------------------------CONTAINER IS SEALED AFTER THIS POINT-------------------------------
// -------------------------------------------------APP IS RUNNING AFTER THIS POINT---------------------------------

app.MapControllers();

// app.UseHttpsRedirection();


app.Run();

