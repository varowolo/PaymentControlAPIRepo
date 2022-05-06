using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentControlAPI.Model
{
    public class PaymentDBContext:DbContext
    {
        public PaymentDBContext()
        {

        }

        public PaymentDBContext(DbContextOptions<PaymentDBContext> options)
            : base(options)
        {

        }
        public virtual DbSet<tblRequestAndReponse> tblRequestAndReponse { get; set; }
        public virtual DbSet<tblEmailLog> tblEmailLog { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tblRequestAndReponse>(entity =>
            {
                entity.ToTable("tblRequestAndReponse");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.RequestType).IsRequired(true).IsUnicode(false).HasMaxLength(100);
                entity.Property(e => e.RequestPayload).IsRequired(true).IsUnicode(true).HasMaxLength(5000);
                entity.Property(e => e.Response).IsRequired(false).IsUnicode(true).HasMaxLength(int.MaxValue);
                entity.Property(e => e.RequestTimestamp).IsRequired(true).HasColumnType("datetime");
                entity.Property(e => e.ResponseTimestamp).IsRequired(true).HasColumnType("datetime");

            });
            modelBuilder.Entity<tblEmailLog>(entity =>
            {
                entity.ToTable("tblEmailLog");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.ToEmail).IsRequired(true).IsUnicode(false).HasMaxLength(100);
                entity.Property(e => e.Subject).IsRequired(true).IsUnicode(true).HasMaxLength(200);
                entity.Property(e => e.Body).IsRequired(true).IsUnicode(false).HasMaxLength(5000);
                entity.Property(e => e.RequestId).IsRequired(true).IsUnicode(false).HasMaxLength(50);
                entity.Property(e => e.Signature).IsRequired(true).IsUnicode(false).HasMaxLength(500);
                entity.Property(e => e.Status).IsRequired(false).IsUnicode(false).HasMaxLength(50);
                entity.Property(e => e.DropTimestamp).IsRequired(true).HasColumnType("datetime");
            });

        }

    }
}
