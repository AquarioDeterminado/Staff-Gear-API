using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace API.src.models;

public partial class AdventureWorksContext : DbContext
{
    public AdventureWorksContext()
    {
    }

    public AdventureWorksContext(DbContextOptions<AdventureWorksContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BusinessEntity> BusinessEntity { get; set; }

    public virtual DbSet<Department> Department { get; set; }

    public virtual DbSet<EmailAddress> EmailAddress { get; set; }

    public virtual DbSet<Employee> Employee { get; set; }

    public virtual DbSet<EmployeeDepartmentHistory> EmployeeDepartmentHistory { get; set; }

    public virtual DbSet<EmployeePayHistory> EmployeePayHistory { get; set; }

    public virtual DbSet<JobCandidate> JobCandidate { get; set; }

    public virtual DbSet<Password> Password { get; set; }

    public virtual DbSet<Person> Person { get; set; }

    public virtual DbSet<Notification> Notification { get; set; }

    public virtual DbSet<Log> Log { get; set; }

    public virtual DbSet<AuthToken> AuthToken { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)  
        => optionsBuilder.UseSqlServer("Server=localhost;Data Source=localhost\\MSSQLSERVER1;Initial Catalog=AdventureWorks2019;TrustServerCertificate=True;persist security info=True;MultipleActiveResultSets=True;User ID=sa;Password=Portugal2025!");
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BusinessEntity>(entity =>
        {
            entity.HasKey(e => e.BusinessEntityID).HasName("PK_BusinessEntity_BusinessEntityID");

            entity.ToTable("BusinessEntity", "Person", tb => tb.HasComment("Source of the ID that connects vendors, customers, and employees with address and contact information."));

            entity.HasIndex(e => e.rowguid, "AK_BusinessEntity_rowguid").IsUnique();

            entity.Property(e => e.BusinessEntityID).HasComment("Primary key for all customers, vendors, and employees.");
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("(getdate())")
                .HasComment("Date and time the record was last updated.")
                .HasColumnType("datetime");
            entity.Property(e => e.rowguid)
                .HasDefaultValueSql("(newid())")
                .HasComment("ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentID).HasName("PK_Department_DepartmentID");

            entity.ToTable("Department", "HumanResources", tb => tb.HasComment("Lookup table containing the departments within the Adventure Works Cycles company."));

            entity.HasIndex(e => e.Name, "AK_Department_Name").IsUnique();

            entity.Property(e => e.DepartmentID).HasComment("Primary key for Department records.");
            entity.Property(e => e.GroupName)
                .HasMaxLength(50)
                .HasComment("Name of the group to which the department belongs.");
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("(getdate())")
                .HasComment("Date and time the record was last updated.")
                .HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasComment("Name of the department.");
        });

        modelBuilder.Entity<EmailAddress>(entity =>
        {
            entity.HasKey(e => new { e.BusinessEntityID, e.EmailAddressID }).HasName("PK_EmailAddress_BusinessEntityID_EmailAddressID");

            entity.ToTable("EmailAddress", "Person", tb => tb.HasComment("Where to send a person email."));

            entity.HasIndex(e => e.EmailAddress1, "IX_EmailAddress_EmailAddress");

            entity.Property(e => e.BusinessEntityID).HasComment("Primary key. Person associated with this email address.  Foreign key to Person.BusinessEntityID");
            entity.Property(e => e.EmailAddressID)
                .ValueGeneratedOnAdd()
                .HasComment("Primary key. ID of this email address.");
            entity.Property(e => e.EmailAddress1)
                .HasMaxLength(50)
                .HasComment("E-mail address for the person.")
                .HasColumnName("EmailAddress");
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("(getdate())")
                .HasComment("Date and time the record was last updated.")
                .HasColumnType("datetime");
            entity.Property(e => e.rowguid)
                .HasDefaultValueSql("(newid())")
                .HasComment("ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.");

            entity.HasOne(d => d.BusinessEntity).WithMany(p => p.EmailAddress)
                .HasForeignKey(d => d.BusinessEntityID)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.BusinessEntityID).HasName("PK_Employee_BusinessEntityID");

            entity.ToTable("Employee", "HumanResources", tb =>
                {
                    tb.HasComment("Employee information such as salary, department, and title.");
                    tb.HasTrigger("dEmployee");
                });

            entity.HasIndex(e => e.LoginID, "AK_Employee_LoginID").IsUnique();

            entity.HasIndex(e => e.NationalIDNumber, "AK_Employee_NationalIDNumber").IsUnique();

            entity.HasIndex(e => e.rowguid, "AK_Employee_rowguid").IsUnique();

            entity.Property(e => e.BusinessEntityID)
                .ValueGeneratedNever()
                .HasComment("Primary key for Employee records.  Foreign key to BusinessEntity.BusinessEntityID.");
            entity.Property(e => e.BirthDate).HasComment("Date of birth.");
            entity.Property(e => e.CurrentFlag)
                .HasDefaultValue(true)
                .HasComment("0 = Inactive, 1 = Active");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("M = Male, F = Female");
            entity.Property(e => e.HireDate).HasComment("Employee hired on this date.");
            entity.Property(e => e.JobTitle)
                .HasMaxLength(50)
                .HasComment("Work title such as Buyer or Sales Representative.");
            entity.Property(e => e.LoginID)
                .HasMaxLength(256)
                .HasComment("Network login.");
            entity.Property(e => e.MaritalStatus)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("M = Married, S = Single");
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("(getdate())")
                .HasComment("Date and time the record was last updated.")
                .HasColumnType("datetime");
            entity.Property(e => e.NationalIDNumber)
                .HasMaxLength(15)
                .HasComment("Unique national identification number such as a social security number.");
            entity.Property(e => e.OrganizationLevel)
                .HasComputedColumnSql("([OrganizationNode].[GetLevel]())", false)
                .HasComment("The depth of the employee in the corporate hierarchy.");
            entity.Property(e => e.SalariedFlag)
                .HasDefaultValue(true)
                .HasComment("Job classification. 0 = Hourly, not exempt from collective bargaining. 1 = Salaried, exempt from collective bargaining.");
            entity.Property(e => e.SickLeaveHours).HasComment("Number of available sick leave hours.");
            entity.Property(e => e.VacationHours).HasComment("Number of available vacation hours.");
            entity.Property(e => e.rowguid)
                .HasDefaultValueSql("(newid())")
                .HasComment("ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.");

            entity.HasOne(d => d.BusinessEntity).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.BusinessEntityID)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<EmployeeDepartmentHistory>(entity =>
        {
            entity.HasKey(e => new { e.BusinessEntityID, e.StartDate, e.DepartmentID, e.ShiftID }).HasName("PK_EmployeeDepartmentHistory_BusinessEntityID_StartDate_DepartmentID");

            entity.ToTable("EmployeeDepartmentHistory", "HumanResources", tb => tb.HasComment("Employee department transfers."));

            entity.HasIndex(e => e.DepartmentID, "IX_EmployeeDepartmentHistory_DepartmentID");

            entity.HasIndex(e => e.ShiftID, "IX_EmployeeDepartmentHistory_ShiftID");

            entity.Property(e => e.BusinessEntityID).HasComment("Employee identification number. Foreign key to Employee.BusinessEntityID.");
            entity.Property(e => e.StartDate).HasComment("Date the employee started work in the department.");
            entity.Property(e => e.DepartmentID).HasComment("Department in which the employee worked including currently. Foreign key to Department.DepartmentID.");
            entity.Property(e => e.ShiftID).HasComment("Identifies which 8-hour shift the employee works. Foreign key to Shift.Shift.ID.");
            entity.Property(e => e.EndDate).HasComment("Date the employee left the department. NULL = Current department.");
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("(getdate())")
                .HasComment("Date and time the record was last updated.")
                .HasColumnType("datetime");

            entity.HasOne(d => d.BusinessEntity).WithMany(p => p.EmployeeDepartmentHistory)
                .HasForeignKey(d => d.BusinessEntityID)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Department).WithMany(p => p.EmployeeDepartmentHistory)
                .HasForeignKey(d => d.DepartmentID)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<EmployeePayHistory>(entity =>
        {
            entity.HasKey(e => new { e.BusinessEntityID, e.RateChangeDate }).HasName("PK_EmployeePayHistory_BusinessEntityID_RateChangeDate");

            entity.ToTable("EmployeePayHistory", "HumanResources", tb => tb.HasComment("Employee pay history."));

            entity.Property(e => e.BusinessEntityID).HasComment("Employee identification number. Foreign key to Employee.BusinessEntityID.");
            entity.Property(e => e.RateChangeDate)
                .HasComment("Date the change in pay is effective")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("(getdate())")
                .HasComment("Date and time the record was last updated.")
                .HasColumnType("datetime");
            entity.Property(e => e.PayFrequency).HasComment("1 = Salary received monthly, 2 = Salary received biweekly");
            entity.Property(e => e.Rate)
                .HasComment("Salary hourly rate.")
                .HasColumnType("money");

            entity.HasOne(d => d.BusinessEntity).WithMany(p => p.EmployeePayHistory)
                .HasForeignKey(d => d.BusinessEntityID)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<JobCandidate>(entity =>
        {
            entity.HasKey(e => e.JobCandidateID).HasName("PK_JobCandidate_JobCandidateID");

            entity.ToTable("JobCandidate", "HumanResources", tb => tb.HasComment("Résumés submitted to Human Resources by job applicants."));

            entity.HasIndex(e => e.BusinessEntityID, "IX_JobCandidate_BusinessEntityID");

            entity.Property(e => e.JobCandidateID).HasComment("Primary key for JobCandidate records.");
            entity.Property(e => e.BusinessEntityID).HasComment("Employee identification number if applicant was hired. Foreign key to Employee.BusinessEntityID.");
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("(getdate())")
                .HasComment("Date and time the record was last updated.")
                .HasColumnType("datetime");
            entity.Property(e => e.Resume)
                .HasComment("Résumé in XML format.")
                .HasColumnType("xml");

            entity.HasOne(d => d.BusinessEntity).WithMany(p => p.JobCandidate).HasForeignKey(d => d.BusinessEntityID);
        });

        modelBuilder.Entity<Password>(entity =>
        {
            entity.HasKey(e => e.BusinessEntityID).HasName("PK_Password_BusinessEntityID");

            entity.ToTable("Password", "Person", tb => tb.HasComment("One way hashed authentication information"));

            entity.Property(e => e.BusinessEntityID).ValueGeneratedNever();
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("(getdate())")
                .HasComment("Date and time the record was last updated.")
                .HasColumnType("datetime");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasComment("Password for the e-mail account.");
            entity.Property(e => e.PasswordSalt)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasComment("Random value concatenated with the password string before the password is hashed.");
            entity.Property(e => e.rowguid)
                .HasDefaultValueSql("(newid())")
                .HasComment("ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.");

            entity.HasOne(d => d.BusinessEntity).WithOne(p => p.Password)
                .HasForeignKey<Password>(d => d.BusinessEntityID)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.BusinessEntityID).HasName("PK_Person_BusinessEntityID");

            entity.ToTable("Person", "Person", tb =>
                {
                    tb.HasComment("Human beings involved with AdventureWorks: employees, customer contacts, and vendor contacts.");
                    tb.HasTrigger("iuPerson");
                });

            entity.HasIndex(e => e.rowguid, "AK_Person_rowguid").IsUnique();

            entity.HasIndex(e => new { e.LastName, e.FirstName, e.MiddleName }, "IX_Person_LastName_FirstName_MiddleName");

            entity.HasIndex(e => e.AdditionalContactInfo, "PXML_Person_AddContact");

            entity.HasIndex(e => e.Demographics, "PXML_Person_Demographics");

            entity.HasIndex(e => e.Demographics, "XMLPATH_Person_Demographics");

            entity.HasIndex(e => e.Demographics, "XMLPROPERTY_Person_Demographics");

            entity.HasIndex(e => e.Demographics, "XMLVALUE_Person_Demographics");

            entity.Property(e => e.BusinessEntityID)
                .ValueGeneratedNever()
                .HasComment("Primary key for Person records.");
            entity.Property(e => e.AdditionalContactInfo)
                .HasComment("Additional contact information about the person stored in xml format. ")
                .HasColumnType("xml");
            entity.Property(e => e.Demographics)
                .HasComment("Personal information such as hobbies, and income collected from online shoppers. Used for sales analysis.")
                .HasColumnType("xml");
            entity.Property(e => e.EmailPromotion).HasComment("0 = Contact does not wish to receive e-mail promotions, 1 = Contact does wish to receive e-mail promotions from AdventureWorks, 2 = Contact does wish to receive e-mail promotions from AdventureWorks and selected partners. ");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasComment("First name of the person.");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasComment("Last name of the person.");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .HasComment("Middle name or middle initial of the person.");
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("(getdate())")
                .HasComment("Date and time the record was last updated.")
                .HasColumnType("datetime");
            entity.Property(e => e.NameStyle).HasComment("0 = The data in FirstName and LastName are stored in western style (first name, last name) order.  1 = Eastern style (last name, first name) order.");
            entity.Property(e => e.PersonType)
                .HasMaxLength(2)
                .IsFixedLength()
                .HasComment("Primary type of person: SC = Store Contact, IN = Individual (retail) customer, SP = Sales person, EM = Employee (non-sales), VC = Vendor contact, GC = General contact");
            entity.Property(e => e.Suffix)
                .HasMaxLength(10)
                .HasComment("Surname suffix. For example, Sr. or Jr.");
            entity.Property(e => e.Title)
                .HasMaxLength(8)
                .HasComment("A courtesy title. For example, Mr. or Ms.");
            entity.Property(e => e.rowguid)
                .HasDefaultValueSql("(newid())")
                .HasComment("ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.");

            entity.HasOne(d => d.BusinessEntity).WithOne(p => p.Person)
                .HasForeignKey<Person>(d => d.BusinessEntityID)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
