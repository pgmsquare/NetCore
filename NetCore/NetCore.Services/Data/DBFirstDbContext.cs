using Microsoft.EntityFrameworkCore;
using NetCore.Data.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Services.Data
{
    public class DBFirstDbContext : DbContext
    {
        public DBFirstDbContext(DbContextOptions<DBFirstDbContext> options) : base(options)
        {
        }

        //DB테이블 리스트 지정
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<UserRolesByUser> UserRolesByUsers { get; set; }

        // virtual
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //DB 테이블이름 변경 및 매핑
            modelBuilder.Entity<User>().ToTable(name: "User");
            modelBuilder.Entity<UserRole>().ToTable(name: "UserRole");
            modelBuilder.Entity<UserRolesByUser>().ToTable(name: "UserRolesByUser");

            //복합키 지정
            modelBuilder.Entity<UserRolesByUser>().HasKey(c => new { c.UserId, c.RoleId });
        }
    }
}
