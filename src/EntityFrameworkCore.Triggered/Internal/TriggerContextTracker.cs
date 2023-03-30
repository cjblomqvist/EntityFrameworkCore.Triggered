using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Triggered.Internal.CascadeStrategies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class TriggerContextTracker
    {
        readonly ChangeTracker _changeTracker;
        readonly ICascadeStrategy _cascadingStrategy;

        List<TriggerContextDescriptor>? _discoveredChanges;
        List<int>? _capturedChangeIndexes;

        public TriggerContextTracker(ChangeTracker changeTracker, ICascadeStrategy cascadingStrategy)
        {
            _changeTracker = changeTracker;
            _cascadingStrategy = cascadingStrategy;
        }

        static ChangeType? ResolveChangeType(EntityEntry entry) => entry.State switch {
            EntityState.Added => ChangeType.Added,
            EntityState.Modified => ChangeType.Modified,
            EntityState.Deleted => ChangeType.Deleted,
            _ => null,
        };

        public IEnumerable<TriggerContextDescriptor>? DiscoveredChanges
        {
            get
            {
                if (_discoveredChanges == null)
                {
                    return null;
                }
                else
                {
                    if (_capturedChangeIndexes == null)
                    {
                        return _discoveredChanges;
                    }
                    else
                    {
                        return _discoveredChanges.Where((_, index) => !_capturedChangeIndexes.Contains(index));
                    }
                }
            }
        }

        public IEnumerable<TriggerContextDescriptor> DiscoverChanges()
        {
            int startIndex;

            _changeTracker.DetectChanges();
            var entries = _changeTracker.Entries();

            if (_discoveredChanges == null)
            {
                _discoveredChanges = new List<TriggerContextDescriptor>(entries.Count());
                startIndex = 0;
            }
            else
            {
                startIndex = _discoveredChanges.Count;
            }

      //var ownedChangedEntries = entries.Where(entry => {
      //  var changeType = ResolveChangeType(entry);

      //  if (changeType is null)
      //    return false;

      //  return entry.Metadata.IsOwned();
      //}).ToList();

      //foreach(var entry in ownedChangedEntries)
      //{
      //  var ownership = entry.Metadata.FindOwnership()!;
      //  var navigation = ownership.GetNavigation(false)!;
      //  var type = navigation.DeclaringType.ClrType;
      //  var propertyName = navigation.Name;
      //  var property = type.GetProperty(propertyName)!;

      //  var newEntries = _changeTracker.Entries();

      //  var ownerTypeEntries = newEntries.Where(e => e.Entity.GetType() == type);
      //  var ownerEntry = ownerTypeEntries.FirstOrDefault(e => property.GetValue(e.Entity) == entry.Entity);

      //  if (ownerEntry is not null && ownerEntry.State == EntityState.Unchanged)
      //  {
      //    ownerEntry.State = EntityState.Modified;
      //  }
      //}

            foreach (var entry in entries)
            {
                var changeType = ResolveChangeType(entry);
                if (changeType != null)
                {
                    if (startIndex > 0)
                    {
                        var canCascade = true;

                        foreach (var discoveredChange in _discoveredChanges)
                        {
                            if (discoveredChange.Entity == entry.Entity)
                            {
                                canCascade = _cascadingStrategy.CanCascade(entry, changeType.Value, discoveredChange);

                                if (!canCascade)
                                {
                                    break;
                                }
                            }
                        }

                        if (!canCascade)
                        {
                            continue;
                        }
                    }

                    var triggerContextDescriptor = new TriggerContextDescriptor(entry, changeType.Value);

                    _discoveredChanges.Add(triggerContextDescriptor!);

            //        if(entry.Metadata.IsOwned())
            //        {
            //var ownership = entry.Metadata.FindOwnership()!;
            //var navigation = ownership.GetNavigation(false)!;
            //var type = navigation.DeclaringType.ClrType;
            //var propertyName = navigation.Name;
            //var property = type.GetProperty(propertyName)!;

            //var newEntries = _changeTracker.Entries();

            //var ownerTypeEntries = newEntries.Where(e => e.Entity.GetType() == type);
            //var ownerEntry = ownerTypeEntries.FirstOrDefault(e => property.GetValue(e.Entity) == entry.Entity);

            //if(ownerEntry is not null && ownerEntry.State == EntityState.Unchanged)
            //{
            //  ownerEntry.State = EntityState.Modified;
            //}
            //        }
                }
            }

            if (startIndex == 0)
            {
                return _discoveredChanges;
            }
            else
            {
                return _discoveredChanges.Skip(startIndex);
            }
        }

        public void CaptureChanges()
        {
            if (_discoveredChanges != null)
            {
                var changesCount = _discoveredChanges.Count;
                if (changesCount > 0)
                {
                    if (_capturedChangeIndexes == null)
                    {
                        _capturedChangeIndexes = new List<int>(changesCount); // assuming all will be captured
                    }

                    for (var changeIndex = 0; changeIndex < changesCount; changeIndex++)
                    {
                        if (!_capturedChangeIndexes.Contains(changeIndex))
                        {
                            var discoveredChange = _discoveredChanges[changeIndex];

                            var currentEntityEntry = _changeTracker.Context.Entry(discoveredChange.Entity);
                            var changeType = ResolveChangeType(currentEntityEntry);

                            if (changeType != discoveredChange.ChangeType)
                            {
                                _capturedChangeIndexes.Add(changeIndex);
                            }
                        }
                    }
                }
            }
        }

        public void UncaptureChanges()
            => _capturedChangeIndexes = null;
    }
}
