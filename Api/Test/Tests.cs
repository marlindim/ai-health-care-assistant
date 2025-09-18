

//using System.ComponentModel.DataAnnotations;
//using System.Text;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;

//#region Core Layer - Entities, DTOs, Interfaces

//public class Patient
//{
//    public Guid Id { get; set; } = Guid.NewGuid();
//    [Required] public string FullName { get; set; } = string.Empty;
//    public DateTime DateOfBirth { get; set; }
//    public string? Email { get; set; }
//    public ICollection<MedicalRecord> Records { get; set; } = new List<MedicalRecord>();
//}

//public class MedicalRecord
//{
//    public Guid Id { get; set; } = Guid.NewGuid();
//    public Guid PatientId { get; set; }
//    public Patient? Patient { get; set; }
//    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//    public string Content { get; set; } = string.Empty;
//    public string? Summary { get; set; }
//}

//public record SymptomCheckRequest(string Symptoms, Guid? PatientId = null);
//public record SymptomCheckResponse(IEnumerable<string> PossibleConditions, string Advice, double Confidence);

//public record MedicalRecordUploadDto(Guid PatientId, string Content);
//public record MedicalRecordSummaryDto(Guid RecordId, string Summary);

//public interface IRepository<T> where T : class
//{
//    Task<T?> GetAsync(Guid id);
//    Task<IEnumerable<T>> GetAllAsync();
//    Task AddAsync(T entity);
//    Task UpdateAsync(T entity);
//    Task DeleteAsync(Guid id);
//}

//public interface IAIClient
//{
//    //A simple abstraction for the Generative AI provider(OpenAI, Azure OpenAI, etc.)
//    Task<string> GenerateTextAsync(string prompt, int maxTokens = 512);
//}

//#endregion

//#region Infrastructure Layer - EF Core, Repos, Encryption

////public class AppDbContext : DbContext
////{
////    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
////    public DbSet<Patient> Patients => Set<Patient>();
////    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
////}

//public class EfRepository<T> : IRepository<T> where T : class
//{
//    private readonly AppDbContext _ctx;
//    private readonly DbSet<T> _dbset;
//    public EfRepository(AppDbContext ctx)
//    {
//        _ctx = ctx;
//        _dbset = ctx.Set<T>();
//    }
//    public async Task AddAsync(T entity)
//    {
//        await _dbset.AddAsync(entity);
//        await _ctx.SaveChangesAsync();
//    }
//    public async Task DeleteAsync(Guid id)
//    {
//        var e = await _dbset.FindAsync(id);
//        if (e == null) return;
//        _dbset.Remove(e);
//        await _ctx.SaveChangesAsync();
//    }
//    public async Task<T?> GetAsync(Guid id)
//    {
//        return await _dbset.FindAsync(id);
//    }
//    public async Task<IEnumerable<T>> GetAllAsync()
//    {
//        return await _dbset.ToListAsync();
//    }
//    public async Task UpdateAsync(T entity)
//    {
//        _dbset.Update(entity);
//        await _ctx.SaveChangesAsync();
//    }
//}

//// Simple encryption service for PHI at rest (demo only)
////public interface IEncryptionService
////{
////    string Encrypt(string plaintext);
////    string Decrypt(string ciphertext);
////}

//public class AesEncryptionService : IEncryptionService
//{
//    private readonly byte[] fg_key;
//    private readonly byte[] _iv;
//    public AesEncryptionService(IConfiguration config)
//    {
//        // For demo use config values - in prod use secure store
//        var keyBase64 = config["Encryption:KeyBase64"] ?? Convert.ToBase64String(Encoding.UTF8.GetBytes("demo-key-32-bytes-length!!"));
//        var ivBase64 = config["Encryption:IVBase64"] ?? Convert.ToBase64String(Encoding.UTF8.GetBytes("demo-iv-16-by"));
//        _key = Convert.FromBase64String(keyBase64);
//        _iv = Convert.FromBase64String(ivBase64);
//    }
//    public string Encrypt(string plaintext)
//    {
//        // NOTE: Demo placeholder - do not use in production as-is.
//        return Convert.ToBase64String(Encoding.UTF8.GetBytes(plaintext));
//    }
//    public string Decrypt(string ciphertext)
//    {
//        var bytes = Convert.FromBase64String(ciphertext);
//        return Encoding.UTF8.GetString(bytes);
//    }
//}

//#endregion

//#region Application Layer - Services and AI wrappers

//public class AiClientStub : IAIClient
//{
//    // This is a stub. Replace with real OpenAI/Azure/Open-source client.
//    public Task<string> GenerateTextAsync(string prompt, int maxTokens = 512)
//    {
//        // Very simple mock response to keep local dev offline-friendly
//        var mock = $"[AI GENERATED SUMMARY]\nPrompt: {prompt.Substring(0, Math.Min(prompt.Length, 200))}...\n(Replace with real AI client)";
//        return Task.FromResult(mock);
//    }
//}

//public class SymptomCheckerService
//{
//    private readonly IAIClient _ai;
//    public SymptomCheckerService(IAIClient ai) { _ai = ai; }

//    public async Task<SymptomCheckResponse> CheckAsync(SymptomCheckRequest req)
//    {
//        // Build prompt for AI
//        var prompt = $"Patient symptoms: {req.Symptoms}\nReturn a JSON array of possible conditions (plain text), short advice, and confidence (0-1).";
//        var aiResult = await _ai.GenerateTextAsync(prompt);

//        // Mock parse - in production parse the structured response
//        var conditions = new List<string> { "Common Cold", "Allergic Rhinitis" };
//        var advice = "Rest, hydrate, and seek medical attention if breathing difficulty or high fever.";
//        var confidence = 0.6;
//        return new SymptomCheckResponse(conditions, advice, confidence);
//    }
//}

//public class MedicalRecordService
//{
//    private readonly IRepository<MedicalRecord> _repo;
//    private readonly IAIClient _ai;
//    public MedicalRecordService(IRepository<MedicalRecord> repo, IAIClient ai)
//    {
//        _repo = repo; _ai = ai;
//    }

//    public async Task<MedicalRecord> UploadRecordAsync(Guid patientId, string content)
//    {
//        var record = new MedicalRecord { PatientId = patientId, Content = content };
//        await _repo.AddAsync(record);
//        return record;
//    }

//    public async Task<MedicalRecord?> SummarizeRecordAsync(Guid recordId)
//    {
//        var record = await _repo.GetAsync(recordId);
//        if (record == null) return null;
//        var prompt = $"Summarize this medical record:\n{record.Content}\nProvide a concise summary with key findings and recommended next steps.";
//        var summary = await _ai.GenerateTextAsync(prompt);
//        record.Summary = summary;
//        await _repo.UpdateAsync(record);
//        return record;
//    }
//}

//#endregion

//#region API Layer - Controllers & Program

//[ApiController]
//[Route("api/[controller]")]
//public class SymptomController : ControllerBase
//{
//    private readonly SymptomCheckerService _service;
//    public SymptomController(SymptomCheckerService service) => _service = service;

//    [HttpPost("check")]
//    public async Task<IActionResult> Check([FromBody] SymptomCheckRequest req)
//    {
//        var res = await _service.CheckAsync(req);
//        return Ok(res);
//    }
//}

//[ApiController]
//[Route("api/[controller]")]
//public class RecordsController : ControllerBase
//{
//    private readonly MedicalRecordService _service;
//    public RecordsController(MedicalRecordService service) => _service = service;

//    [HttpPost("upload")]
//    public async Task<IActionResult> Upload([FromBody] MedicalRecordUploadDto dto)
//    {
//        var rec = await _service.UploadRecordAsync(dto.PatientId, dto.Content);
//        return CreatedAtAction(nameof(Get), new { id = rec.Id }, rec);
//    }

//    [HttpPost("summarize/{id}")]
//    public async Task<IActionResult> Summarize(Guid id)
//    {
//        var rec = await _service.SummarizeRecordAsync(id);
//        if (rec == null) return NotFound();
//        return Ok(new MedicalRecordSummaryDto(rec.Id, rec.Summary ?? string.Empty));
//    }

//    [HttpGet("{id}")]
//    public async Task<IActionResult> Get(Guid id)
//    {
//        // For demo, use the DbContext directly would be better to use repo
//        return Ok();
//    }
//}

//public static class Program
//{
//    public static void Main(string[] args)
//    {
//        var builder = WebApplication.CreateBuilder(args);
//        var services = builder.Services;
//        var configuration = builder.Configuration;

//        // Add DbContext - using in-memory for demo; replace with SQL Server/Postgres in prod
//        services.AddDbContext<AppDbContext>(opts => opts.UseInMemoryDatabase("AIHealthcareDb"));

//        // Register repositories
//        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

//        // Register services and AI client stub
//        services.AddSingleton<IAIClient, AiClientStub>();
//        services.AddScoped<SymptomCheckerService>();
//        services.AddScoped<MedicalRecordService>();

//        // Encryption
//        services.AddSingleton<IEncryptionService, AesEncryptionService>();

//        // Authentication - JWT (simple config)
//        var jwtKey = configuration["Jwt:Key"] ?? "replace-this-secret-with-secure-key";
//        var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
//        services.AddAuthentication(options =>
//        {
//            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//        }).AddJwtBearer(options =>
//        {
//            options.RequireHttpsMetadata = false;
//            options.SaveToken = true;
//            options.TokenValidationParameters = new TokenValidationParameters
//            {
//                ValidateIssuer = false,
//                ValidateAudience = false,
//                ValidateLifetime = true,
//                ValidateIssuerSigningKey = true,
//                IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
//            };
//        });

//        services.AddAuthorization();

//        services.AddControllers();
//        services.AddEndpointsApiExplorer();
//        services.AddSwaggerGen();

//        var app = builder.Build();
//        if (app.Environment.IsDevelopment())
//        {
//            app.UseSwagger();
//            app.UseSwaggerUI();
//        }

//        app.UseAuthentication();
//        app.UseAuthorization();

//        app.MapControllers();

//        // Seed demo data
//        using (var scope = app.Services.CreateScope())
//        {
//            var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//            if (!ctx.Patients.Any())
//            {
//                ctx.Patients.Add(new Patient { FullName = "Demo Patient", Email = "demo@example.com", DateOfBirth = DateTime.UtcNow.AddYears(-30) });
//                ctx.SaveChanges();
//            }
//        }

//        app.Run();
//    }
//}

//#endregion

//#region Tests (xUnit) - Example (illustrative, not runnable in this single-file)


//public class SymptomCheckerTests
//{
//    [Fact]
//    public async Task Check_Returns_Response()
//    {
//        var ai = new AiClientStub();
//        var svc = new SymptomCheckerService(ai);
//        var res = await svc.CheckAsync(new SymptomCheckRequest("cough and fever"));
//        Assert.NotNull(res);
//        Assert.True(res.PossibleConditions.Any());
//    }
//}


//#endregion

