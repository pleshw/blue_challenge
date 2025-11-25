using BlueChallenge.Api.Model.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlueChallenge.Api.Data.Configurations;

public class UserModelConfiguration : IEntityTypeConfiguration<UserModel>
{
    public void Configure(EntityTypeBuilder<UserModel> builder)
    {
        builder.HasKey(user => user.Id);

        builder.OwnsOne(user => user.Credentials, creds =>
        {
            creds.OwnsOne(c => c.Email, email =>
            {
                email.Property(e => e.Alias).HasMaxLength(120);
                email.Property(e => e.Provider).HasMaxLength(120);
                email.Ignore(e => e.Address);
            });
            creds.Property(c => c.Password).HasMaxLength(100);
        });

        builder.OwnsOne(user => user.PersonalInfo, info =>
        {
            info.Property(i => i.Name).HasMaxLength(120);

            info.OwnsOne(i => i.Phone, phone =>
            {
                phone.Property(p => p.DDD).HasMaxLength(2);
                phone.Property(p => p.Number).HasMaxLength(9);
            });

            info.OwnsMany(i => i.Emails, emails =>
            {
                emails.WithOwner().HasForeignKey("UserId");
                emails.Property(e => e.Alias).HasMaxLength(120);
                emails.Property(e => e.Provider).HasMaxLength(120);
                emails.Ignore(e => e.Address);
                emails.Property<int>("Id");
                emails.HasKey("Id");
            });
        });
    }
}
