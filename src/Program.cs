#region ConfigureServices

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

builder.Services.AddDbContext<Context>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentityEntityFrameworkContextConfiguration(options => 
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), 
	b=>b.MigrationsAssembly("MinimalApi")));

builder.Services.AddIdentityConfiguration();
builder.Services.AddJwtConfiguration(builder.Configuration)
        .AddNetDevPackIdentity<IdentityUser>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin",
        policy => policy.RequireClaim("Admin"));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Minimal API",
        Description = "Developed by Marcelo",
        Contact = new OpenApiContact { Name = "Marcelo Rosario", Email = "marcelorosario2001@gmail.com" },
        License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT desta maneira: Bearer {seu token}",
        Name = "Authorization",
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


var app = builder.Build();

#endregion

#region ConfigurePipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthConfiguration();
app.UseAuthorization();
app.UseHttpsRedirection();

EndPointIdentity(app);
EndPointProvider(app);
EndPointCategory(app);
EndPointProduct(app);

app.Run();

#endregion

#region EndPoints
void EndPointIdentity(WebApplication app){

app.MapPost("/registro", [AllowAnonymous] async (SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IOptions<AppJwtSettings> appJwtSettings, RegisterUser registerUser ) => { 

if(registerUser == null)
return Results.BadRequest("Usuario não informado");

if(!MiniValidator.TryValidate(registerUser, out var errors))
return Results.ValidationProblem(errors);

var user = new IdentityUser{

UserName = registerUser.Email,
Email = registerUser.Email,
EmailConfirmed = true

};

var result = await userManager.CreateAsync(user, registerUser.Password);

if(!result.Succeeded)
return Results.BadRequest(result.Errors);

var jwt = new JwtBuilder()
.WithUserManager(userManager)
.WithJwtSettings(appJwtSettings.Value)
.WithEmail(user.Email)
.WithJwtClaims()
.WithUserClaims()
.WithUserRoles()
.BuildUserResponse();

return Results.Ok(jwt);

}).ProducesValidationProblem()
.Produces<RegisterUser>(StatusCodes.Status200OK) 
.Produces<RegisterUser>(StatusCodes.Status400BadRequest)
.WithName("RegistroUsuario").WithTags("Usuario");


app.MapPost("/login", [AllowAnonymous] async ( SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IOptions<AppJwtSettings> appJwtSettings, LoginUser loginUser) => {

if(loginUser == null)
return Results.BadRequest("Usuario não informado");

if(!MiniValidator.TryValidate(loginUser, out var errors))
return Results.ValidationProblem(errors);

var result = await signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

if(result.IsLockedOut)
return Results.BadRequest("Usuario bloqueado");


if(!result.Succeeded)
return Results.BadRequest("Usuario ou senha invalidos");


var jwt = new JwtBuilder()
.WithUserManager(userManager)
.WithJwtSettings(appJwtSettings.Value)
.WithEmail(loginUser.Email)
.WithJwtClaims()
.WithUserClaims()
.WithUserRoles()
.BuildUserResponse();


return Results.Ok(jwt);


}).ProducesValidationProblem()
.Produces<LoginUser>(StatusCodes.Status200OK) 
.Produces<LoginUser>(StatusCodes.Status400BadRequest)
.WithName("LoginUsuario").WithTags("Usuario");

}

void EndPointProvider(WebApplication app){



app.MapGet("/Provider", [AllowAnonymous] async (Context context) => {

return Results.Ok(await context.Provider.ToListAsync());

}).Produces<ProvidersOutput>(StatusCodes.Status200OK)
 .Produces<ProvidersOutput>(StatusCodes.Status404NotFound)
 .WithName("GetProvider")
 .WithTags("Provider");


app.MapGet("/Provider/{id}", [AllowAnonymous] async (Context context, int id) => {

return await context.Provider.FindAsync(id)
is Provider provider ? Results.Ok(new ProvidersOutput(provider.Id ,provider.Name, provider.Active)) : Results.NotFound();

}).Produces<ProvidersOutput>(StatusCodes.Status200OK)
 .Produces<ProvidersOutput>(StatusCodes.Status404NotFound)
 .WithName("GetProviderById")
 .WithTags("Provider");



app.MapPost("/Provider", [Authorize] async (Context context, ProvidersInput model) => {

if (!MiniValidator.TryValidate(model, out var errors))
return Results.ValidationProblem(errors);

var provider = new Provider{

Id = model.Id,
Name = model.Name,
Active = model.Active,

};

context.Provider.Add(provider);
var result = await context.SaveChangesAsync();

return result > 0? Results.CreatedAtRoute($"/Category/{provider.Id}", new ProvidersOutput(provider.Id, provider.Name, provider.Active)): Results.BadRequest("Houve um problema ao salvar o registro");

}).ProducesValidationProblem()
 .Produces<ProvidersOutput>(StatusCodes.Status201Created) 
 .Produces<ProvidersOutput>(StatusCodes.Status400BadRequest)
 .RequireAuthorization("Admin")
 .WithName("PostProvider")
 .WithTags("Provider");


app.MapPut("/Provider/{id}", [Authorize] async( int id, Context context, ProvidersInput model) => {

var providerDb = await context.Provider.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id); 

if(providerDb == null)
return Results.NotFound();

if (!MiniValidator.TryValidate(model, out var errors))
return Results.ValidationProblem(errors);


var provider = new Provider{

Id = model.Id,
Name = model.Name,
Active = model.Active,

};

context.Provider.Update(provider);
var result = await context.SaveChangesAsync();


return result > 0? Results.NoContent():Results.BadRequest("Houve um problema ao salvar o registro");


}).ProducesValidationProblem()
.Produces<ProvidersOutput>(StatusCodes.Status204NoContent) 
.Produces<ProvidersOutput>(StatusCodes.Status400BadRequest)
.RequireAuthorization("Admin")
.WithName("PutProvider").WithTags("Provider");


app.MapDelete("/Provider/{id}", [Authorize] async( int id, Context context) => {

var Provider = await context.Provider.FindAsync(id); 

if(Provider == null) 
return Results.NotFound();

context.Provider.Remove(Provider);
var result = await context.SaveChangesAsync();


return result > 0? Results.NoContent():Results.BadRequest("Houve um problema ao salvar o registro");


}).ProducesValidationProblem()
.Produces<ProvidersOutput>(StatusCodes.Status204NoContent) 
.Produces<ProvidersOutput>(StatusCodes.Status400BadRequest)
.RequireAuthorization("Admin")
.WithName("DeleteProvider").WithTags("Provider");

}

void EndPointCategory(WebApplication app){

app.MapGet("/Category", [AllowAnonymous] async (Context context) => {

return Results.Ok(await context.Category.ToListAsync());

}).Produces<CategoriesOutput>(StatusCodes.Status200OK)
 .Produces<CategoriesOutput>(StatusCodes.Status404NotFound)
 .WithName("GetCategory")
 .WithTags("Category");

app.MapGet("/Category/{id}", [AllowAnonymous] async (Context context, int id) =>

await context.Category.FindAsync(id)
is Category category ? Results.Ok(new CategoriesOutput(category.Id, category.Title)) : Results.NotFound()

).Produces<CategoriesOutput>(StatusCodes.Status200OK)
 .Produces<CategoriesOutput>(StatusCodes.Status404NotFound)
 .WithName("GetCategoryById")
 .WithTags("Category");

app.MapPost("/Category", [Authorize] async (Context context, CategoriesInput model) => {

if (!MiniValidator.TryValidate(model, out var errors))
return Results.ValidationProblem(errors);

var category = new Category{
Id = model.Id,
Title = model.Title,
};


context.Category.Add(category);
var result = await context.SaveChangesAsync();

return result > 0? Results.Created($"/Category/{category.Id}", new CategoriesOutput(category.Id, category.Title)): Results.BadRequest("Houve um problema ao salvar o registro");

}).ProducesValidationProblem()
 .Produces<CategoriesOutput>(StatusCodes.Status201Created) 
 .Produces<CategoriesOutput>(StatusCodes.Status400BadRequest)
 .RequireAuthorization("Admin")
 .WithName("PostCategory")
 .WithTags("Category");

 app.MapPut("/Category/{id}", [Authorize] async( int id, Context context, CategoriesInput model) => {

var CategoryDb = await context.Category.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id); 

if(CategoryDb == null)
return Results.NotFound();

if (!MiniValidator.TryValidate(model, out var errors))
return Results.ValidationProblem(errors);


var category = new Category{
Id = model.Id,
Title = model.Title,
};

context.Category.Update(category);
var result = await context.SaveChangesAsync();


return result > 0? Results.NoContent():Results.BadRequest("Houve um problema ao salvar o registro");


}).ProducesValidationProblem()
.Produces<CategoriesOutput>(StatusCodes.Status204NoContent) 
.Produces<CategoriesOutput>(StatusCodes.Status400BadRequest)
.RequireAuthorization("Admin")
.WithName("PutCategory").WithTags("Category");


app.MapDelete("/Category/{id}", [Authorize] async( int id, Context context) => {

var category = await context.Category.FindAsync(id); 

if(category == null) 
return Results.NotFound();

context.Category.Remove(category);
var result = await context.SaveChangesAsync();


return result > 0? Results.NoContent():Results.BadRequest("Houve um problema ao salvar o registro");


}).ProducesValidationProblem()
.Produces<CategoriesOutput>(StatusCodes.Status204NoContent) 
.Produces<CategoriesOutput>(StatusCodes.Status400BadRequest)
.RequireAuthorization("Admin")
.WithName("DeleteCategory").WithTags("Category");

}

void EndPointProduct(WebApplication app){


    app.MapGet("/Product", [AllowAnonymous] async (Context context) => {

var product = await context.Product
                           .AsNoTracking()
                           .Select(x => new ListProductsViewModel {

                             Id = x.Id,
                             Title = x.Title,
                             Price = x.Price,
                             LastUpdateDate = x.InputDate,
                             Provider = x.Provider.Name,
                             Category = x.Category.Title

                         }).ToListAsync();

return Results.Ok(product);


}).Produces<ProductsOutput>(StatusCodes.Status200OK)
 .Produces<ProductsOutput>(StatusCodes.Status404NotFound)
 .WithName("GetProduct")
 .WithTags("Product");


app.MapGet("/Product/{id}", [AllowAnonymous] async (Context context, int id) => {

var product = await context.Product
                           .AsNoTracking()
                           .Where(x => x.Id == id)
                           .Select(x => new ListProductsViewModel {

                             Id = x.Id,
                             Title = x.Title,
                             Price = x.Price,
                             LastUpdateDate = x.InputDate,
                             Provider = x.Provider.Name,
                             Category = x.Category.Title

                         }).FirstOrDefaultAsync();

return Results.Ok(product);

}).Produces<ProductsOutput>(StatusCodes.Status200OK)
  .Produces<ProductsOutput>(StatusCodes.Status404NotFound)
  .WithName("GetProductById")
  .WithTags("Product");



app.MapPost("/Product", [Authorize] async (Context context, ProductsInput model) => {

if (!MiniValidator.TryValidate(model, out var errors))
return Results.ValidationProblem(errors);

var product = new Product{

Id = model.Id,
Title = model.Title,
Price = model.Price,
InputDate = DateTime.Now,
ProviderId = model.ProviderId,
CategoryId = model.CategoryId,

};

context.Product.Add(product);
var result = await context.SaveChangesAsync();

return result > 0? Results.Created($"Product/{product.Id}", new ProductsOutput(product.Id, product.Title, product.Price, product.InputDate, product.ProviderId, product.CategoryId)): Results.BadRequest("Houve um problema ao salvar o registro");

}).ProducesValidationProblem()
 .Produces<ProductsOutput>(StatusCodes.Status204NoContent) 
 .Produces<ProductsOutput>(StatusCodes.Status400BadRequest)
 .RequireAuthorization("Admin")
 .WithName("PostProduct")
 .WithTags("Product");

app.MapPut("/Product/{id}", [Authorize] async( int id, Context context, ProductsInput model) => {

var product = await context.Product
                             .AsNoTracking()
                             .FirstOrDefaultAsync(x => x.Id == id); 

if(product == null)
return Results.NotFound();

if (!MiniValidator.TryValidate(model, out var errors))
return Results.ValidationProblem(errors);

product.Id = model.Id;
product.Title = model.Title;
product.Price = model.Price;
product.InputDate = DateTime.Now;
product.ProviderId = model.ProviderId;
product.CategoryId = model.CategoryId;

context.Product.Update(product);
var result = await context.SaveChangesAsync();

return result > 0? Results.NoContent():Results.BadRequest("Houve um problema ao salvar o registro");

}).ProducesValidationProblem()
.Produces<ProductsOutput>(StatusCodes.Status204NoContent) 
.Produces<ProductsOutput>(StatusCodes.Status400BadRequest)
.RequireAuthorization("Admin")
.WithName("PutProduct").WithTags("Product");


app.MapDelete("/Products/{id}", [Authorize] async( int id, Context context) => {

var products = await context.Product.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id); 

if(products == null) 
return Results.NotFound();

context.Product.Remove(products);
var result = await context.SaveChangesAsync();

return result > 0? Results.NoContent():Results.BadRequest("Houve um problema ao salvar o registro");

}).ProducesValidationProblem()
.Produces<ProductsOutput>(StatusCodes.Status204NoContent) 
.Produces<ProductsOutput>(StatusCodes.Status400BadRequest)
.RequireAuthorization("Admin")
.WithName("DeleteProduct").WithTags("Product");

}

#endregion