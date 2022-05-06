//using System;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentControlAPI.Models
{
    public partial class PaymentControlDBContext : DbContext
    {
        public PaymentControlDBContext()
        {
        }

        public PaymentControlDBContext(DbContextOptions<PaymentControlDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblCustNotificationLog> TblCustNotificationLog { get; set; }
        public virtual DbSet<TblEmailLog> TblEmailLog { get; set; }
        public virtual DbSet<TblRequestAndReponse> TblRequestAndReponse { get; set; }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer("Server=.\\MSSQLSERVER2K14;Database=PaymentControlDB;User Id=localuser; Password=Password123;");
//            }
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblCustNotificationLog>(entity =>
            {
                entity.ToTable("tblCustNotificationLog");

                entity.HasIndex(e => e.RequestId)
                    .HasName("UC_RequestId")
                    .IsUnique();

                entity.Property(e => e.AccountNumber)
                    .IsRequired()
                    .HasColumnName("accountNumber")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.ControlId)
                    .HasColumnName("controlId")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDate)
                    .IsRequired()
                    .HasColumnName("createDate")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EmailDestination)
                    .HasColumnName("emailDestination")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NotificationDatetime)
                    .HasColumnName("notificationDatetime")
                    .HasColumnType("datetime");

                entity.Property(e => e.NotificationMessage)
                    .IsRequired()
                    .HasColumnName("notificationMessage")
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.NotificationStatus)
                    .HasColumnName("notificationStatus")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.NotificationType)
                    .IsRequired()
                    .HasColumnName("notificationType")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RequestId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Signature)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<TblEmailLog>(entity =>
            {
                entity.ToTable("tblEmailLog");

                entity.Property(e => e.Body)
                    .IsRequired()
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.DropTimestamp).HasColumnType("datetime");

                entity.Property(e => e.EmailDatetime)
                    .HasColumnName("emailDatetime")
                    .HasColumnType("datetime");

                entity.Property(e => e.EmailStatus)
                    .HasColumnName("emailStatus")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.RequestId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Signature)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ToEmail)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TblRequestAndReponse>(entity =>
            {
                entity.ToTable("tblRequestAndReponse");

                entity.Property(e => e.RequestPayload).IsRequired();

                entity.Property(e => e.RequestTimestamp).HasColumnType("datetime");

                entity.Property(e => e.RequestType)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ResponseTimestamp).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
