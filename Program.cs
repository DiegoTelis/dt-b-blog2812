using Blog;
using Blog.Data;
using Blog.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IO.Compression;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
ConfigureAuthentication(builder);
ConfigureMvc(builder);
ConfigureService(builder);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

        //redireciona o http  para https  da pra fazer isso nos serviços tbm
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCompression();

LoadConfiguration(app);



app.UseStaticFiles();   // para fazer  o servidor renderizar arquivos staticos, css js e imagem ? 
                        // Usando staticFiles o servidor SEMPRE vai procurar a pasta wwwroot na raiz do programa
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

void LoadConfiguration(WebApplication app)
{
    //No appSettings
    //obtem connectionString
    //app.Configuration.GetConnectionString("");
    //retorna o valor que esta dentro dessa propriedade jwtKey
    //app.Configuration.GetValue<string>("JwtKey");
    //obter da appSettings uma seção como pegar a SMTP-classe faz o parso para a classe
    //app.Configuration.GetSection("Smtp");

    Configuration.JWTKey = app.Configuration.GetValue<string>("JWTKey");
    Configuration.ApiKeyName = app.Configuration.GetValue<string>("ApiKeyName");
    Configuration.ApiKey = app.Configuration.GetValue<string>("ApiKey");

    var smtp = new Configuration.SmtpConfiguration();
    app.Configuration.GetSection("Smtp").Bind(smtp);
    Configuration.Smtp = smtp;
}

void ConfigureAuthentication(WebApplicationBuilder builder)
{
    var key = Encoding.ASCII.GetBytes(Configuration.JWTKey);
    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

}

void ConfigureMvc(WebApplicationBuilder builder)
{
    //Para poder usar cache, evitando hit's no banco sem necessidade - usado na CategoryController
    builder.Services.AddMemoryCache();  // apenas essa linha na config é necessaria

    // para poder usar compressão de dados, diminuir o tamanho na transição
    builder.Services.AddResponseCompression(options =>
    {           // As opções de compressões 
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
        //options.Providers.Add<CustomCompressionProvider>();  // essa Não encontrou o nome mas tem outra
    });

    builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Optimal;
    });
    // Lembrar de usar o app.UseResponseCompression();



    builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>     ///
    {       // Esta parte vai configurar definindo que o ASP.NET não execute o Model State automático. 
        options.SuppressModelStateInvalidFilter = true;   //Precisa fazer manual agora
    })
    .AddJsonOptions(x =>   //  parte adicionada para acabar com a redundancia das consultas......
    {     //  1 categoria tem varios posts o Post tem uma categoria e ficaria repetindo, com isto acaba

        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
    });

}

void ConfigureService(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultaConnection");

    //config do dbContext
    builder.Services.AddDbContext<BlogDataContext>( options => options.UseSqlServer(connectionString));

    builder.Services.AddTransient<TokenService>();
    //builder.Services.AddScoped();       // Cria um novo a cada Requisição - se acaso passar por X metodos que usa o mesmo,
    //                                    // vai aproveitar

    //builder.Services.AddTransient();    // Sempres usar um novo a cada uso da um new 
    // Singleton                          // 1 por produto.
    ///------------------
    ///

    builder.Services.AddTransient<EmailService>();


}




