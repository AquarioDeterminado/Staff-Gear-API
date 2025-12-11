using System;
using System.Collections.Generic;

namespace API.src.models;

/// <summary>
/// Résumés submitted to Human Resources by job applicants.
/// </summary>
public partial class JobCandidate
{
    /// <summary>
    /// Primary key for JobCandidate records.
    /// </summary>
    public int JobCandidateID { get; set; }

    /// <summary>
    /// Employee identification number if applicant was hired. Foreign key to Employee.BusinessEntityID.
    /// </summary>
    public int? BusinessEntityID { get; set; }

    /// <summary>
    /// Résumé in XML format.
    /// </summary>
    public string? Resume { get; set; }

    /// <summary>
    /// Date and time the record was last updated.
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    public required string FirstName { get; set; }

    public string MiddleName { get; set; } = "";

    public string LastName { get; set; } = "";

    public required string Email { get; set; }

    public required string Message { get; set; }

    public string Phone { get; set; } = "";

    public virtual Employee? BusinessEntity { get; set; }
}
