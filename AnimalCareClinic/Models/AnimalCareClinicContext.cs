using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AnimalCareClinic.Models;

public partial class AnimalCareClinicContext : DbContext
{
    public AnimalCareClinicContext()
    {
    }

    public AnimalCareClinicContext(DbContextOptions<AnimalCareClinicContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Animal> Animals { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Owner> Owners { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }
    public virtual DbSet<UserAccount> UserAccounts { get; set; }


    public virtual DbSet<Veterinarian> Veterinarians { get; set; }

    public virtual DbSet<VisitHistory> VisitHistories { get; set; }

    public virtual DbSet<VwVetCalendar> VwVetCalendars { get; set; }

    public virtual DbSet<VwVisitSummary> VwVisitSummaries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=ZENBOOK-NP;Database=AnimalCareClinic;Trusted_Connection=True;TrustServerCertificate=True;");


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Animal>(entity =>
        {
            entity.HasKey(e => e.AnimalId).HasName("PK__Animal__A21A7327EE40EFBC");

            entity.ToTable("Animal");

            entity.HasIndex(e => e.OwnerId, "IX_Animal_Owner");

            entity.Property(e => e.AnimalId).HasColumnName("AnimalID");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MedicalHistory)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.Species)
                .HasMaxLength(40)
                .IsUnicode(false);

            entity.HasOne(d => d.Owner).WithMany(p => p.Animals)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Animal_Owner");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCA225FC6CEA");

            entity.ToTable("Appointment");

            entity.HasIndex(e => e.AnimalId, "IX_Appt_Animal");

            entity.HasIndex(e => e.ScheduleId, "IX_Appt_Schedule");

            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.AnimalId).HasColumnName("AnimalID");
            entity.Property(e => e.AppointmentTime).HasPrecision(0);
            entity.Property(e => e.Reason)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("scheduled");

            entity.HasOne(d => d.Animal).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.AnimalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appt_Animal");

            entity.HasOne(d => d.Schedule).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appt_Schedule");
        });

        modelBuilder.Entity<Owner>(entity =>
        {
            entity.HasKey(e => e.OwnerId).HasName("PK__Owner__81938598CD6BE0BC");

            entity.ToTable("Owner");

            entity.HasIndex(e => e.Email, "UQ_Owner_Email").IsUnique();

            entity.HasIndex(e => e.PhoneNumber, "UQ_Owner_Phone").IsUnique();

            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Schedule__9C8A5B69B9D56080");

            entity.ToTable("Schedule");

            entity.HasIndex(e => new { e.VeterinarianId, e.Date }, "IX_Schedule_Vet_Date");

            entity.HasIndex(e => new { e.VeterinarianId, e.Date, e.TimeSlot }, "UQ_Schedule").IsUnique();

            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TimeSlot)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.VeterinarianId).HasColumnName("VeterinarianID");

            entity.HasOne(d => d.Veterinarian).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.VeterinarianId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Vet");
        });
        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_UserAccount");

            entity.ToTable("UserAccount");

            entity.HasIndex(e => e.Username).IsUnique();

            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Role).HasMaxLength(20);
        });


        modelBuilder.Entity<Veterinarian>(entity =>
        {
            entity.HasKey(e => e.VeterinarianId).HasName("PK__Veterina__C97D02D8F80EF5BE");

            entity.ToTable("Veterinarian");

            entity.HasIndex(e => e.Email, "UQ_Vet_Email").IsUnique();

            entity.HasIndex(e => e.PhoneNumber, "UQ_Vet_Phone").IsUnique();

            entity.Property(e => e.VeterinarianId).HasColumnName("VeterinarianID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Speciality)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VisitHistory>(entity =>
        {
            entity.HasKey(e => e.VisitId).HasName("PK__VisitHis__4D3AA1BE2C67E3D0");

            entity.ToTable("VisitHistory");

            entity.HasIndex(e => new { e.AnimalId, e.VeterinarianId, e.VisitDate }, "IX_Visit_Animal_VetDate");

            entity.HasIndex(e => e.AppointmentId, "IX_Visit_Appt");

            entity.Property(e => e.VisitId).HasColumnName("VisitID");
            entity.Property(e => e.AnimalId).HasColumnName("AnimalID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.Diagnosis)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Prescription)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Treatment)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.VeterinarianId).HasColumnName("VeterinarianID");

            entity.HasOne(d => d.Animal).WithMany(p => p.VisitHistories)
                .HasForeignKey(d => d.AnimalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Visit_Animal");

            entity.HasOne(d => d.Appointment).WithMany(p => p.VisitHistories)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Visit_Appt");

            entity.HasOne(d => d.Veterinarian).WithMany(p => p.VisitHistories)
                .HasForeignKey(d => d.VeterinarianId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Visit_Vet");
        });

        modelBuilder.Entity<VwVetCalendar>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_VetCalendar");

            entity.Property(e => e.AnimalId).HasColumnName("AnimalID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.AppointmentStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.SlotStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TimeSlot)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Veterinarian)
                .HasMaxLength(101)
                .IsUnicode(false);
            entity.Property(e => e.VeterinarianId).HasColumnName("VeterinarianID");
        });

        modelBuilder.Entity<VwVisitSummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_VisitSummary");

            entity.Property(e => e.AnimalId).HasColumnName("AnimalID");
            entity.Property(e => e.AnimalName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Diagnosis)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.OwnerName)
                .HasMaxLength(101)
                .IsUnicode(false);
            entity.Property(e => e.Prescription)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Treatment)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.VetName)
                .HasMaxLength(101)
                .IsUnicode(false);
            entity.Property(e => e.VeterinarianId).HasColumnName("VeterinarianID");
            entity.Property(e => e.VisitId).HasColumnName("VisitID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
