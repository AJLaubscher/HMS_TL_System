using System;
using HMS_API.Dtos.userAccount;
using HMS_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace HMS_API.Data;

public class HMS_Context(DbContextOptions<HMS_Context> options) : DbContext (options)
{
    public DbSet<UserAccount> UserAccounts {get; set;}
    public DbSet<Module> Modules {get; set;}
    public DbSet<Enrolment> Enrolments {get; set;}
    public DbSet<Assignment> Assignments {get; set;}
    public DbSet<Submission> Submissions {get; set;}
    public DbSet<Feedback> Feedbacks {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccount>().ToTable("userAccount"); //name change

        // relationships in database

        // one lecturer > many modules
        modelBuilder.Entity<Module>()
        .ToTable("module")
        .HasOne(m => m.UserAccount)                 // one user
        .WithMany(u => u.Modules)                   // many modules
        .HasForeignKey(m => m.LectID);              // point to user(lecturer)

        // enrolment composite keys, one user > many enrolments, one module > many enrolments
        modelBuilder.Entity<Enrolment>()
        .ToTable("enrolment")
        .HasKey( e => new {e.ModID, e.StudID}); // composite key
        
        modelBuilder.Entity<Enrolment>()
        .HasOne(e => e.UserAccount)                     // one user
        .WithMany(u => u.Enrolments)                    // many enrollments
        .HasForeignKey(e => e.StudID);                  // point to student

        modelBuilder.Entity<Enrolment>()
        .HasOne(e => e.Module)                          // one module
        .WithMany(m => m.Enrolments)                    // many enrollments
        .HasForeignKey(e => e.ModID);                   // point to module


        // one module > many assignments
        modelBuilder.Entity<Assignment>()
        .ToTable("assignment")
        .HasOne(m => m.Module)                          // one module
        .WithMany(a => a.Assignments)                   // many assignments
        .HasForeignKey(m => m.ModID);                   // point to module

        // one assignment > many submissions
        modelBuilder.Entity<Submission>()
        .ToTable("submission")
        .HasOne(a => a.Assignment)                      // one assignment
        .WithMany(s => s.Submissions)                   // many submissions
        .HasForeignKey(s => s.AssignID);                // point to assignment

        // one student > many submissions
        modelBuilder.Entity<Submission>()
        .ToTable("submission")
        .HasOne(u => u.UserAccount)                    // one user(student)
        .WithMany(s => s.Submissions)                   //  many submisson
        .HasForeignKey(u => u.StudID);                // point to User


        modelBuilder.Entity<Feedback>()
        .HasOne(e => e.UserAccount)                     // one user
        .WithMany(u => u.Feedbacks)                    // many feedback
        .HasForeignKey(e => e.LectID);                  // point to lecturer

        modelBuilder.Entity<Feedback>()
        .ToTable("feedback")
        .HasOne(f => f.Submission)                  //one submission
        .WithMany(s => s.Feedbacks)                 // more than one feedback
        .HasForeignKey(f => f.SubID);              //point to submission

    }
}
