using Microsoft.EntityFrameworkCore;
using HitTrackerAPI.Models;

namespace HitTrackerAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Tablolar
        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<WorkoutProgram> WorkoutPrograms { get; set; }
        public DbSet<ProgramExercise> ProgramExercises { get; set; }
        public DbSet<WorkoutSession> WorkoutSessions { get; set; }
        public DbSet<SessionExercise> SessionExercises { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User — email unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // User — username unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // UserProfile — bir kullanıcının bir profili olabilir
            modelBuilder.Entity<UserProfile>()
                .HasIndex(p => p.UserId)
                .IsUnique();

            // UserProfile → User ilişkisi
            modelBuilder.Entity<UserProfile>()
                .HasOne(p => p.User)
                .WithOne(u => u.Profile)
                .HasForeignKey<UserProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Exercise — isim unique
            modelBuilder.Entity<Exercise>()
                .HasIndex(e => e.Name)
                .IsUnique();

            // WorkoutProgram → User
            modelBuilder.Entity<WorkoutProgram>()
                .HasOne(p => p.User)
                .WithMany(u => u.Programs)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProgramExercise → Program
            modelBuilder.Entity<ProgramExercise>()
                .HasOne(pe => pe.Program)
                .WithMany(p => p.Exercises)
                .HasForeignKey(pe => pe.ProgramId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProgramExercise → Exercise
            modelBuilder.Entity<ProgramExercise>()
                .HasOne(pe => pe.Exercise)
                .WithMany()
                .HasForeignKey(pe => pe.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);

            // WorkoutSession → User
            modelBuilder.Entity<WorkoutSession>()
                .HasOne(s => s.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // SessionExercise → Session
            modelBuilder.Entity<SessionExercise>()
                .HasOne(se => se.Session)
                .WithMany(s => s.Exercises)
                .HasForeignKey(se => se.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // SessionExercise → Exercise
            modelBuilder.Entity<SessionExercise>()
                .HasOne(se => se.Exercise)
                .WithMany()
                .HasForeignKey(se => se.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}