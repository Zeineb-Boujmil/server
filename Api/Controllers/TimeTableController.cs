using Api.Controllers.Abstract;
using DataAccess;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using Api.Attributes;
using Api.Constants;

namespace Api.Controllers
{
    public class TimeTableController : BaseController<TimeTable>
    {
        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public override IQueryable<TimeTable> Get()
        {
            return Context.Set<TimeTable>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public override SingleResult<TimeTable> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<TimeTable>().Where(e => e.Id == key));
        }

        [HttpPost]
        //[Auth(AuthActionTypes.Create, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Post(TimeTable entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (entity.AppointmentTimeSlots != null && entity.AppointmentTimeSlots.Any())
                foreach (var appointmentTimeSlot in entity.AppointmentTimeSlots)
                {
                    appointmentTimeSlot.TimeTableId = entity.Id;
                    Context.AppointmentTimeSlots.Add(appointmentTimeSlot);
                    Context.Entry(appointmentTimeSlot.Product).State = EntityState.Detached;
                }

            if (entity.TimeTableSlots != null && entity.TimeTableSlots.Any())
                Context.TimeTableSlots.AddRange(entity.TimeTableSlots);

            entity.Id = Guid.NewGuid();

            Context.TimeTables.Add(entity);
            await Context.SaveChangesAsync();

            return Created(entity);
        }

        [HttpPut]
        //[Auth(AuthActionTypes.Update, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Put(Guid key, TimeTable entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (key == Guid.Empty || key != entity.Id || !Context.TimeTables.Any(e => e.Id == key))
            {
                return BadRequest("Invalid key ");
            }

            CRUD(entity.AppointmentTimeSlots,
               await Context.AppointmentTimeSlots.AsNoTracking().Where(x => x.TimeTableId == key).ToListAsync(),
               ats =>
               {
                   ats.TimeTableId = key;
                   Context.AppointmentTimeSlots.Add(ats);
                   Context.Entry(ats.Product).State = EntityState.Detached;
               },
               ats => { },
               ats =>
               {
                   Context.Entry(ats).State = EntityState.Deleted;
               });

            CRUD(entity.TimeTableSlots,
                await Context.TimeTableSlots.AsNoTracking().Where(x => x.TimeTableId == key).ToListAsync(),
                tts =>
                {
                    tts.TimeTableId = key;
                    Context.TimeTableSlots.Add(tts);
                },
                tts =>
                {
                    Context.Entry(tts.TimeSlot).State = EntityState.Modified;
                    Context.Entry(tts).State = EntityState.Modified;
                },
                tts =>
                {
                    Context.Entry(tts.TimeSlot).State = EntityState.Deleted;
                    Context.Entry(tts).State = EntityState.Deleted;
                });

            Context.Entry(entity).State = EntityState.Modified;
            await Context.SaveChangesAsync();

            return Updated(entity);
        }
    }
}
