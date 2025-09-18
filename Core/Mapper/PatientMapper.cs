using Core.Dtos;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Mapper
{
    public static class PatientMapper
    {
        public static PatientDto ToDto(Patient patient)
        {
            return new PatientDto(
                patient.Id,
                patient.FullName,
                patient.DateOfBirth,
                //patient.Age,
                patient.Email
            );
        }

        public static Patient ToEntity(CreatePatientDto dto)
        {
            return new Patient
            {
                FullName = dto.FullName,
                DateOfBirth = dto.DateOfBirth,
                Email = dto.Email
            };
        }

        public static void UpdateEntity(Patient patient, UpdatePatientDto dto)
        {
            patient.FullName = dto.FullName;
            patient.DateOfBirth = dto.DateOfBirth;
            patient.Email = dto.Email;
        }
    }
}
