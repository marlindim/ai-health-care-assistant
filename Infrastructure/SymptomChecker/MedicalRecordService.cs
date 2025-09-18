using Core.Contract;
using Core.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.SymptomChecker
{
    public class MedicalRecordService
    {
        private readonly IEncryptionService _encryption;
        private readonly HealthcareDb _db;

        public MedicalRecordService(IEncryptionService encryption, HealthcareDb db)
        {
            _encryption = encryption;
            _db = db;
        }

        // Save and encrypt a record
        public async Task<MedicalRecord> SaveRecordAsync(string plainRecord, Guid PatientI)
        {
            var encryptedData = _encryption.Encrypt(plainRecord);
            var entity = new MedicalRecord
            {
                EncryptedContent = encryptedData,
                PatientId = PatientI

            };

            await _db.MedicalRecords.AddAsync(entity);
            await _db.SaveChangesAsync();

            entity.Content = plainRecord; // Optionally set the plain content for immediate use
            return entity;
        }

        // Retrieve and decrypt a record
        public async Task<string?> GetRecordAsync(Guid id)
        {
            var record = await _db.MedicalRecords.FindAsync(id);
            if (record == null) return null;

            return _encryption.Decrypt(record.EncryptedContent);
        }

        // Summarize, encrypt, save summary, and return updated record
        public async Task<MedicalRecord?> SummarizeAndSaveRecordAsync(Guid id, IAIClient aiService)
        {
            var record = await _db.MedicalRecords.FindAsync(id);
            if (record == null) return null;

            var decrypted = _encryption.Decrypt(record.EncryptedContent);
            var summary = await aiService.SummarizeAsync(decrypted);

            record.EncryptedSummary = _encryption.Encrypt(summary);

            //_db.MedicalRecords.Update(record);
            record.EncryptedSummary = _encryption.Encrypt(summary);
            await _db.SaveChangesAsync();
             record.Content = decrypted; // Optionally set the plain content for immediate use
            record.Summary = summary; // Optionally set the plain summary for immediate use

            return record;
        }

        // Retrieve and decrypt a record summary
        public async Task<string?> GetSummaryAsync(Guid id)
        {
            var record = await _db.MedicalRecords.FindAsync(id);
            if (record == null || string.IsNullOrEmpty(record.EncryptedSummary)) return null;

            return _encryption.Decrypt(record.EncryptedSummary);
        }
    }
}