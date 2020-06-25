namespace Chearn.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web.Configuration;

    public partial class ChearnContext : DbContext
    {
        public ChearnContext()

           :base(WebConfigurationManager.AppSettings.Get("Context"))
           // :base(WebConfigurationManager.AppSettings.Get("Context"))

           //:base("AzureDevContext")
        {
        }

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Cours> Courses { get; set; }
        public virtual DbSet<CUser> CUsers { get; set; }
        public virtual DbSet<Edge> Edges { get; set; }
        public virtual DbSet<Instructor> Instructors { get; set; }
        public virtual DbSet<Lesson> Lessons { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<StudentLesson> StudentLessons { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<ItemTag> CourseTags { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<TagScore> TagScores { get; set; }
        public virtual DbSet<ShopItem> ShopItems { get; set; }
        public virtual DbSet<OwnedItem> OwnedItems { get; set; }
        public virtual DbSet<ShopItemCours> ShopItemCourses { get; set; }
        public virtual DbSet<CourseInstructor> CourseInstructors { get; set; }
        public virtual DbSet<BlogPost> BlogPosts { get; set; } 

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.CUsers)
                .WithOptional(e => e.AspNetRole)
                .HasForeignKey(e => e.AspID);

            modelBuilder.Entity<Cours>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Cours>()
                .HasMany(e => e.Lessons)
                .WithOptional(e => e.Cours)
                .HasForeignKey(e => e.CourseID);

            modelBuilder.Entity<CUser>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<CUser>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<CUser>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<CUser>()
                .HasMany(e => e.UserRoles)
                .WithOptional(e => e.CUser)
                .HasForeignKey(e => e.UserID);

            modelBuilder.Entity<Lesson>()
                .Property(e => e.MaterialA)
                .IsUnicode(false);

            modelBuilder.Entity<Lesson>()
                .Property(e => e.MaterialB)
                .IsUnicode(false);

            modelBuilder.Entity<Lesson>()
                .HasMany(e => e.Edges)
                .WithOptional(e => e.Lesson)
                .HasForeignKey(e => e.ChildID);

            modelBuilder.Entity<Lesson>()
                .HasMany(e => e.Edges1)
                .WithOptional(e => e.Lesson1)
                .HasForeignKey(e => e.ParentID);

            modelBuilder.Entity<Question>()
                .Property(e => e.Text)
                .IsUnicode(false);

            modelBuilder.Entity<Question>()
                .Property(e => e.Answer)
                .IsUnicode(false);

            modelBuilder.Entity<Role>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Category>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<ItemTag>()
                .Property(e => e.Tag)
                .IsUnicode(false);

            modelBuilder.Entity<TagScore>()
                .Property(e => e.Tag0)
                .IsUnicode(false);

            modelBuilder.Entity<TagScore>()
                .Property(e => e.Tag1)
                .IsUnicode(false);

            modelBuilder.Entity<ShopItem>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<ShopItem>()
                .Property(e => e.Description)
                .IsUnicode(false);
        }

        public System.Data.Entity.DbSet<Chearn.Models.Enrollment> Enrollments { get; set; }
    }
}
