using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Interfaces;

namespace DataAccess
{
    public partial class MasterDataContext
    {
        public override int SaveChanges()
        {
	        try
	        {
		        SetCommonData();
		        return base.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				var errmsg = new StringBuilder();
				foreach (var eve in e.EntityValidationErrors)
				{
					errmsg.AppendFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
						eve.Entry.Entity.GetType().Name, eve.Entry.State);
					foreach (var ve in eve.ValidationErrors)
					{
						errmsg.AppendFormat("- Property: \"{0}\", Error: \"{1}\"",
							ve.PropertyName, ve.ErrorMessage);
					}
				}
				throw new ApplicationException(errmsg.ToString(), e);
			}
		}

        public override Task<int> SaveChangesAsync()
        {
            SetCommonData();
            return base.SaveChangesAsync();
        }

        private void SetCommonData()
        {
            var userName = ClaimsPrincipal.Current?.FindFirst(ClaimTypes.Name)?.Value ?? "N/A";

            foreach (var entity in ChangeTracker.Entries().Where(e => e.State == EntityState.Added).Select(e => e.Entity))
            {
                if (entity is IIdentifiable<Guid> identifiable)
                {
                    if (identifiable.Id == Guid.Empty)
                        identifiable.Id = Guid.NewGuid();
                }

                if (entity is ITrackable trackable)
                {
                    trackable.CreatedDate = DateTime.UtcNow;
                    trackable.CreatedBy = userName;
                    trackable.LastModifiedDate = DateTime.UtcNow;
                    trackable.LastModifiedBy = userName;
                }
            }

            foreach (var entity in ChangeTracker.Entries().Where(e => e.State == EntityState.Modified).Select(e => e.Entity))
                if (entity is ITrackable trackable)
                {
                    trackable.LastModifiedDate = DateTime.UtcNow;
                    trackable.LastModifiedBy = userName;
                }
        }
    }
}