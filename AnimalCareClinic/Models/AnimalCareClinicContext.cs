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

    public virtual DbSet<Veterinarian> Veterinarians { get; set; }

    public virtual DbSet<VisitHistory> VisitHistories { get; set; }

    public virtual DbSet<VwVetCalendar> VwVetCalendars { get; set; }

    public virtual DbSet<VwVisitSummary> VwVisitSummaries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=ZENBOOK-NP;Database=AnimalCareClinic;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Animal>(entity =>
        {
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
                .HasConstraintName("FK_Animal_Owner");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
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
                .HasConstraintName("FK_Schedule_Vet");
        });

        modelBuilder.Entity<Veterinarian>(entity =>
        {
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
            entity.HasKey(e => e.VisitId);

            entity.ToTable("VisitHistory");

            entity.HasIndex(e => new { e.AnimalId, e.VeterinarianId, e.VisitDate }, "IX_Visit_Animal_Vet_Date");

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
