using BlueChallenge.Api.Data;
using BlueChallenge.Api.Model.Schedule;
using Microsoft.EntityFrameworkCore;

namespace BlueChallenge.Api.Repository;

public class ScheduleRepository : IScheduleRepository
{
    private readonly BlueChallengeDbContext _context;

    public ScheduleRepository(BlueChallengeDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(ScheduleModel schedule, CancellationToken cancellationToken = default)
    {
        await _context.Schedules.AddAsync(schedule, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<ScheduleModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Schedules
            .AsNoTracking()
            .Include(schedule => schedule.User)
            .FirstOrDefaultAsync(schedule => schedule.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<ScheduleModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var schedules = await _context.Schedules
            .AsNoTracking()
            .Include(schedule => schedule.User)
            .ToListAsync(cancellationToken);

        return schedules;
    }

    public async Task UpdateAsync(ScheduleModel schedule, CancellationToken cancellationToken = default)
    {
        _context.Schedules.Update(schedule);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        if (schedule is null)
        {
            return false;
        }

        _context.Schedules.Remove(schedule);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
