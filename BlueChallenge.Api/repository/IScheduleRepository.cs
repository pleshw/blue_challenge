using BlueChallenge.Api.Model.Schedule;

namespace BlueChallenge.Api.Repository;

public interface IScheduleRepository
{
    Task CreateAsync(ScheduleModel schedule, CancellationToken cancellationToken = default);
    Task<ScheduleModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ScheduleModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(ScheduleModel schedule, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
