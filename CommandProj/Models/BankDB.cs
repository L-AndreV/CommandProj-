using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.SqlServer;


namespace CommandProj.Models
{
    public class BankContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<CreditHistory> CreditHistories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<ClientAuthData> ClientAuthData { get; set; }
        public DbSet<EmployeeAuthData> EmployeeAuthData { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<CreditStatement> CreditStatements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);
            modelBuilder.Entity<Employee>()
                .HasKey(e => e.EmployeeId);
            modelBuilder.Entity<Account>()
                .HasKey(a => a.AccountId);
            modelBuilder.Entity<Loan>()
                .HasKey(l => l.LoanId);
            modelBuilder.Entity<Deposit>()
                .HasKey(d => d.DepositId);
            modelBuilder.Entity<CreditHistory>()
                .HasKey(ch => ch.RecordId);
            modelBuilder.Entity<Transaction>()
                .HasKey(t => t.TransactionId);
            modelBuilder.Entity<Branch>()
                .HasKey(b => b.BranchId);
            modelBuilder.Entity<CreditStatement>()
                .HasKey(cs => cs.StatementId);
            modelBuilder.Entity<ClientAuthData>()
                .HasKey(ca => ca.Phone);
            modelBuilder.Entity<EmployeeAuthData>()
                .HasKey(ea => ea.Phone);
            modelBuilder.Entity<Country>()
                .HasKey(c => c.CountryId);

            modelBuilder.Entity<User>().HasIndex(e => e.Phone).IsUnique();

            modelBuilder.Entity<Employee>().HasIndex(e => e.Phone).IsUnique();

            modelBuilder.Entity<User>()
                .HasOne<ClientAuthData>()
                .WithOne()
                .HasForeignKey<User>(ca => ca.Phone);

            modelBuilder.Entity<Employee>()
                .HasOne<EmployeeAuthData>()
                .WithOne()
                .HasForeignKey<Employee>(ea => ea.Phone);

            modelBuilder.Entity<Employee>()
                .HasOne<Branch>()
                .WithMany()
                .HasForeignKey(e => e.BranchId);

            modelBuilder.Entity<Account>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(a => a.UserId);

            modelBuilder.Entity<Loan>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(l => l.UserId);

            modelBuilder.Entity<Deposit>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(d => d.UserId);

            modelBuilder.Entity<CreditHistory>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(ch => ch.UserId);

            //modelBuilder.Entity<Transaction>()
            //    .HasOne<User>(t => t.Recipient)
            //    .WithMany()
            //    .HasForeignKey(t => t.RecipientId).OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<Transaction>()
            //    .HasOne<User>(t => t.Sender)
            //    .WithMany()
            //    .HasForeignKey(t => t.SenderId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transaction>()
                .HasOne<Account>()
                .WithMany()
                .HasForeignKey(t => t.RecipientAccountId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Transaction>()
                .HasOne<Account>()//t => t.SenderAccount)
                .WithMany()
                .HasForeignKey(t => t.SenderAccountId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CreditStatement>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(cs => cs.UserId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CreditStatement>()
                .HasOne<Branch>()
                .WithMany()
                .HasForeignKey(cs => cs.BranchId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Branch>()
                .HasMany<Account>()
                .WithOne()
                .HasForeignKey(b => b.AccountId);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-STO09GR\\SQLEXPRESS;Initial Catalog=BankApp;Integrated Security=True;Encrypt=False");
            //optionsBuilder.UseSqlite("Data Source=Tv1.db");
        }
    }
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
    }
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccessLevel { get; set; }
        public string Phone { get; set; }
        public int BranchId { get; set; }
        public string Country { get; set; }
        public DateTime HireDate { get; set; }
    }

    public class Account //Это счета
    {
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; }
        //public Transaction Transaction { get; set; }
    }

    public class Loan //Это кредиты
    {
        public int LoanId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime IssueDate { get; set; }
    }

    public class Deposit //Это вклады
    {
        public int DepositId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
    }

    public class CreditHistory
    {
        public int RecordId { get; set; }
        public int UserId { get; set; }
        public int CurrentLoansCount { get; set; }
        public int RepaidLoansCount { get; set; }
        public decimal AverageLoanSize { get; set; }
    }

    public class Transaction
    {
        public int TransactionId { get; set; }
        //public int RecipientId { get; set; }
        //public int SenderId { get; set; }
        public int RecipientAccountId { get; set; }
        public int SenderAccountId { get; set; }
        public decimal Amount { get; set; }
        public string MessageToRecipient { get; set; }
        public DateTime Date { get; set; }

        //public User Recipient { get; set; }
        //public User Sender { get; set; }
        //public Account RecipientAccount { get; set; }
        //public Account SenderAccount { get; set; }
    }

    public class Branch
    {
        public int BranchId { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public int AccountId { get; set; }
    }

    public class CreditStatement
    {
        public int StatementId { get; set; }
        public int UserId { get; set; }
        public int BranchId { get; set; }
        public int Amount { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
    }

    public class ClientAuthData
    {
        public string Phone { get; set; }
        public string Password { get; set; }
    }

    public class EmployeeAuthData
    {
        public string Phone { get; set; }
        public string Password { get; set; }
    }

    public class Country
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string PhoneCode { get; set; }
    }
}
