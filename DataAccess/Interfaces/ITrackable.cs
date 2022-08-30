using System;

namespace DataAccess.Interfaces
{
    public interface ITrackable
    {
        DateTime CreatedDate { get; set; }
        string CreatedBy { get; set; }
        DateTime LastModifiedDate { get; set; }
        string LastModifiedBy { get; set; }
    }
}