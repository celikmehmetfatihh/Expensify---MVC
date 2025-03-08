using Expensify.Web.Models;
using Expensify.Web.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Expensify.Web.Data
{
    public class ExpensifyDbContext : IdentityDbContext<ApplicationUser>
    {
        public ExpensifyDbContext(DbContextOptions<ExpensifyDbContext> options) : base(options)
        {
        }

        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseForm> ExpenseForms { get; set; }
        public DbSet<ExpenseAction> ExpenseActions { get; set; }
        public DbSet<ExpenseFormHistory> ExpenseFormHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var adminRoleId = "4b78a717-2022-4b5f-8661-680e0fc56425";
            var managerRoleId = "7f27476a-810d-4a07-8201-fd4694ab42d7";
            var accountantRoleId = "2f7fddf7-9048-40ea-9cb6-66d5213e992c";
            var employeeRoleId = "8bd0d5f7-15b0-4ea8-b770-67583c415ccd";

            var roles = new List<IdentityRole>()
            {
                new IdentityRole()
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = adminRoleId
                },
                new IdentityRole()
                {
                    Id = managerRoleId,
                    Name = "Manager",
                    NormalizedName = "MANAGER",
                    ConcurrencyStamp = managerRoleId
                },
                new IdentityRole()
                {
                    Id = accountantRoleId,
                    Name = "Accountant",
                    NormalizedName = "ACCOUNTANT",
                    ConcurrencyStamp = accountantRoleId
                },
                new IdentityRole()
                {
                    Id = employeeRoleId,
                    Name = "Employee",
                    NormalizedName = "EMPLOYEE",
                    ConcurrencyStamp = employeeRoleId
                },
            };

            builder.Entity<IdentityRole>().HasData(roles);

            // Seed Users
            var adminId = "bc6d91c1-5b6f-4a1b-a4bb-c5df30af9367";
            var managerId1 = "d3b05818-1fa7-4adf-861b-94f8376a4cc1";
            var managerId2 = "e5b9c6d1-9fcd-4ad2-8e8e-567fd20e2a9b";
            var accountantId = "e8a0dc0e-6c32-4a87-93b5-b05439ef05d1";
            var employee1Id = "23f1b176-d123-4d55-95be-b7d353c77de9";
            var employee2Id = "cc69ff21-9b3a-4d50-9a14-71c34e31b9e4";
            var employee3Id = "1fbb4387-2f8b-4d59-bdb7-8475c5cf2e93";

            var passwordHasher = new PasswordHasher<ApplicationUser>();

            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = adminId,
                    UserName = "admin@expensify.com",
                    NormalizedUserName = "ADMIN@EXPENSIFY.COM",
                    Email = "admin@expensify.com",
                    NormalizedEmail = "ADMIN@EXPENSIFY.COM",
                    EmailConfirmed = true,
                    FullName = "Rajnish Pathania",
                },
                new ApplicationUser
                {
                    Id = managerId1,
                    UserName = "manager1@expensify.com",
                    NormalizedUserName = "MANAGER1@EXPENSIFY.COM",
                    Email = "manager1@expensify.com",
                    NormalizedEmail = "MANAGER1@EXPENSIFY.COM",
                    EmailConfirmed = true,
                    FullName = "Kenan Tas",
                },
                new ApplicationUser
                {
                    Id = managerId2,
                    UserName = "manager2@expensify.com",
                    NormalizedUserName = "MANAGER2@EXPENSIFY.COM",
                    Email = "manager2@expensify.com",
                    NormalizedEmail = "MANAGER2@EXPENSIFY.COM",
                    EmailConfirmed = true,
                    FullName = "Aziz Ullah",
                },
                new ApplicationUser
                {
                    Id = accountantId,
                    UserName = "accountant@expensify.com",
                    NormalizedUserName = "ACCOUNTANT@EXPENSIFY.COM",
                    Email = "accountant@expensify.com",
                    NormalizedEmail = "ACCOUNTANT@EXPENSIFY.COM",
                    EmailConfirmed = true,
                    FullName = "Dusan Tadic",
                },
                new ApplicationUser
                {
                    Id = employee1Id,
                    UserName = "employee1@expensify.com",
                    NormalizedUserName = "EMPLOYEE1@EXPENSIFY.COM",
                    Email = "employee1@expensify.com",
                    NormalizedEmail = "EMPLOYEE1@EXPENSIFY.COM",
                    EmailConfirmed = true,
                    FullName = "Mehmet Fatih Celik",
                    ManagerId = managerId1,  // This employee's manager
                },
                new ApplicationUser
                {
                    Id = employee2Id,
                    UserName = "employee2@expensify.com",
                    NormalizedUserName = "EMPLOYEE2@EXPENSIFY.COM",
                    Email = "employee2@expensify.com",
                    NormalizedEmail = "EMPLOYEE2@EXPENSIFY.COM",
                    EmailConfirmed = true,
                    FullName = "Ata Kaleli",
                    ManagerId = managerId1,  // This employee's manager
                },
                new ApplicationUser
                {
                    Id = employee3Id,
                    UserName = "employee3@expensify.com",
                    NormalizedUserName = "EMPLOYEE3@EXPENSIFY.COM",
                    Email = "employee3@expensify.com",
                    NormalizedEmail = "EMPLOYEE3@EXPENSIFY.COM",
                    EmailConfirmed = true,
                    FullName = "Hasan Eren Yarar",
                    ManagerId = managerId2,  // This employee's manager
                }
            };

            // Hash passwords after initializing users
            foreach (var user in users)
            {
                user.PasswordHash = passwordHasher.HashPassword(user, "Password@123");
            }

            builder.Entity<ApplicationUser>().HasData(users);

            // Seed User-Role Relationships
            var userRoles = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string> { UserId = adminId, RoleId = adminRoleId },
                new IdentityUserRole<string> { UserId = managerId1, RoleId = managerRoleId },
                new IdentityUserRole<string> { UserId = managerId2, RoleId = managerRoleId },
                new IdentityUserRole<string> { UserId = accountantId, RoleId = accountantRoleId },
                new IdentityUserRole<string> { UserId = employee1Id, RoleId = employeeRoleId },
                new IdentityUserRole<string> { UserId = employee2Id, RoleId = employeeRoleId },
                new IdentityUserRole<string> { UserId = employee3Id, RoleId = employeeRoleId },
            };

            builder.Entity<IdentityUserRole<string>>().HasData(userRoles);

            Guid expenseForm1Id = Guid.Parse("43d5acaf-fd22-4bc8-a944-f46cfa3f377e");
            Guid expenseForm2Id = Guid.Parse("2cd4fdbc-cba7-4860-b2c4-f9e402aa7235");
            Guid expenseForm3Id = Guid.Parse("fb101135-d92f-4cea-b713-febf3a098f04");
            Guid expenseForm4Id = Guid.Parse("9546ec67-e5ff-4e21-97e2-62875fe50ac2");
            Guid expenseForm5Id = Guid.Parse("f34dc252-46c5-4708-82d8-705414c0c4c4");

            var expenseForms = new List<ExpenseForm>
    {
        new ExpenseForm
        {
            Id = expenseForm1Id,
            SubmissionDate = new DateTime(2024, 9, 10),
            EmployeeId = "23f1b176-d123-4d55-95be-b7d353c77de9",  // employee1
            ManagerId = "d3b05818-1fa7-4adf-861b-94f8376a4cc1",    // manager1
            Currency = "TL",
            AccountantId = accountantId,
            IsApprovedByManager = false,
            IsPaidByAccountant = false,
            IsRejectedByManager = false,
            RejectionReason = null
        },
        new ExpenseForm
        {
            Id = expenseForm2Id,
            SubmissionDate = new DateTime(2024, 9, 12),
            EmployeeId = "23f1b176-d123-4d55-95be-b7d353c77de9",  // employee1
            ManagerId = "d3b05818-1fa7-4adf-861b-94f8376a4cc1",    // manager1
            Currency = "USD",
            AccountantId = accountantId,
            IsApprovedByManager = false,
            IsPaidByAccountant = false,
            IsRejectedByManager = false,
            RejectionReason = null
        },
        new ExpenseForm
        {
            Id = expenseForm3Id,
            SubmissionDate = new DateTime(2024, 9, 15),
            EmployeeId = "cc69ff21-9b3a-4d50-9a14-71c34e31b9e4",  // employee2
            ManagerId = "d3b05818-1fa7-4adf-861b-94f8376a4cc1",    // manager1
            Currency = "USD",
            AccountantId = accountantId,
            IsApprovedByManager = false,
            IsPaidByAccountant = false,
            IsRejectedByManager = false,
            RejectionReason = null
        },
        new ExpenseForm
        {
            Id = expenseForm4Id,
            SubmissionDate = new DateTime(2024, 9, 18),
            EmployeeId = "cc69ff21-9b3a-4d50-9a14-71c34e31b9e4",  // employee2
            ManagerId = "d3b05818-1fa7-4adf-861b-94f8376a4cc1",    // manager1
            Currency = "EUR",
            AccountantId = accountantId,
            IsApprovedByManager = false,
            IsPaidByAccountant = false,
            IsRejectedByManager = false,
            RejectionReason = null
        },
        new ExpenseForm
        {
            Id = expenseForm5Id,
            SubmissionDate = new DateTime(2024, 9, 20),
            EmployeeId = "1fbb4387-2f8b-4d59-bdb7-8475c5cf2e93",  // employee3
            ManagerId = "e5b9c6d1-9fcd-4ad2-8e8e-567fd20e2a9b",    // manager2
            Currency = "USD",
            AccountantId = accountantId,
            IsApprovedByManager = false,
            IsPaidByAccountant = false,
            IsRejectedByManager = false,
            RejectionReason = null
        }
    };

            builder.Entity<ExpenseForm>().HasData(expenseForms);

            // Seed Expenses with foreign key references to ExpenseForms
            var expenses = new List<Expense>
    {
        new Expense
        {
            Id = Guid.NewGuid(),
            Description = "Office Supplies",
            Amount = 120.50M,
            Date = new DateTime(2024, 9, 5),
            ExpenseFormId = expenseForm1Id
        },
        new Expense
        {
            Id = Guid.NewGuid(),
            Description = "Business Lunch",
            Amount = 45.75M,
            Date = new DateTime(2024, 9, 10),
            ExpenseFormId = expenseForm2Id
        },
        new Expense
        {
            Id = Guid.NewGuid(),
            Description = "Conference Fees",
            Amount = 300.00M,
            Date = new DateTime(2024, 9, 11),
            ExpenseFormId = expenseForm2Id
        },
        new Expense
        {
            Id = Guid.NewGuid(),
            Description = "Software License",
            Amount = 450.00M,
            Date = new DateTime(2024, 9, 14),
            ExpenseFormId = expenseForm3Id
        },
        new Expense
        {
            Id = Guid.NewGuid(),
            Description = "Hotel Accommodation",
            Amount = 675.19M,
            Date = new DateTime(2024, 9, 18),
            ExpenseFormId = expenseForm4Id
        },
        new Expense
        {
            Id = Guid.NewGuid(),
            Description = "Taxi Fare",
            Amount = 25.00M,
            Date = new DateTime(2024, 9, 19),
            ExpenseFormId = expenseForm4Id
        },
        new Expense
        {
            Id = Guid.NewGuid(),
            Description = "Client Meeting Dinner",
            Amount = 150.00M,
            Date = new DateTime(2024, 9, 19),
            ExpenseFormId = expenseForm5Id
        },
        new Expense
        {
            Id = Guid.NewGuid(),
            Description = "Stationery Purchase",
            Amount = 50.00M,
            Date = new DateTime(2024, 9, 20),
            ExpenseFormId = expenseForm5Id
        },
        new Expense
        {
            Id = Guid.NewGuid(),
            Description = "Flight Ticket",
            Amount = 800.00M,
            Date = new DateTime(2024, 9, 22),
            ExpenseFormId = expenseForm5Id
        }
    };

            builder.Entity<Expense>().HasData(expenses);

            // Seed Action table with initial data
            var actions = new List<ExpenseAction>
            {
                new ExpenseAction { Id = Guid.Parse("cabd35ab-c820-4ba7-9054-8aaf85213e8e"),
                    Name = "Created" },
                new ExpenseAction { Id = Guid.Parse("0c8332db-8304-4cfb-bbd5-3b568cd8142a"),
                    Name = "Rejected" },
                new ExpenseAction { Id = Guid.Parse("db5641fd-1d42-471b-a525-63cb1b264ae5"),
                    Name = "Approved" },
                new ExpenseAction { Id = Guid.Parse("6c00baa4-8b4f-4170-b58f-800464817a1e"),
                    Name = "Updated" },
                new ExpenseAction { Id = Guid.Parse("d1626ffb-9d52-427f-9daa-adc270788e57"),
                    Name = "Paid" }
            };

            builder.Entity<ExpenseAction>().HasData(actions);

            // Configure foreign key relationships for ExpenseForm
            builder.Entity<ExpenseForm>()
                .HasOne(e => e.Employee)
                .WithMany()
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ExpenseForm>()
                .HasOne(e => e.Manager)
                .WithMany()
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ExpenseForm>()
                .HasOne(e => e.Accountant)
                .WithMany()
                .HasForeignKey(e => e.AccountantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ExpenseFormHistory>()
                 .HasOne(h => h.ExpenseForm)
                 .WithMany()
                 .HasForeignKey(h => h.ExpenseFormId)
                 .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ExpenseFormHistory>()
                .HasOne(h => h.Action)
                .WithMany()
                .HasForeignKey(h => h.ActionId)
                .OnDelete(DeleteBehavior.Cascade);

			builder.Entity<ExpenseFormHistory>()
	            .HasOne(e => e.User)
	            .WithMany()
	            .HasForeignKey(e => e.PerformedByUserId)
	            .OnDelete(DeleteBehavior.Restrict);


		}
    }
}
