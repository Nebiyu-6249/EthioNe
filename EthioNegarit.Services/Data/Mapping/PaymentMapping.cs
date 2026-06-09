using EthioNegarit.Core.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace EthioNegarit.Services.Data.Mapping
{
    public class PaymentMapping : EntityTypeConfiguration<Payment>
    {
        public PaymentMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.Month).IsRequired();
            Property(x => x.Reference).IsRequired();           
            Property(x => x.InvoiceNo).IsRequired();
            Property(x => x.Discount).IsRequired();
            Property(x => x.TotalPrice).IsRequired();
            Property(x => x.Amount).IsRequired();
            Property(x => x.DateTransfer).IsRequired();
            Property(x => x.DateSubmitted).IsRequired();
            Property(x => x.DateEdited).IsOptional();
            Property(x => x.Bank).IsOptional();
            Property(x => x.AccountNo).IsOptional();
            Property(x => x.PaymentMode).IsOptional();           
            Property(x => x.DateConfirmed).IsRequired();
            Property(x => x.ConfirmedBy).IsOptional();

            Property(x => x.Comment).IsOptional();
            Property(x => x.PaymentStatus).IsRequired();

             HasRequired(x => x.Package)
    .WithMany(x => x.Payments)
    .Map(x => x.MapKey("Package_Id"))
    .WillCascadeOnDelete(false);

            HasRequired(x => x.User)
    .WithMany(x => x.Payments)
    .Map(x => x.MapKey("MembershipUser_Id"))
    .WillCascadeOnDelete(false);

            HasRequired(x => x.Advertisement)
      .WithMany(x => x.Payments)
      .Map(x => x.MapKey("Advertisement_Id"))
      .WillCascadeOnDelete(false);
        }
        }
}
