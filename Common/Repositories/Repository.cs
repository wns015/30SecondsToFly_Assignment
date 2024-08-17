using Common.Contexts;
using Common.Contexts.Models;
using Common.Logs;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Caching;
using System.ComponentModel.DataAnnotations;

namespace Common.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseModel
    { 
    
        private readonly TSFContext dbCtx;
        private readonly string ctxID;
        public const string EntityConfigAdd = "EF_NoDetectChange_Add";
        public const string EntityConfigUpdate = "EF_NoDetectChange_Update";
        public const string EntityConfigDelete = "EF_NoDetectChange_Delete";
        public const string EntityConfigCache = "EF_NoDetectChange_Cache";
        private readonly string CacheIDKey = "30secondstofly_db_cache_key";
        private readonly DbSet<TEntity> dbSet;

        private MemoryCache Cache => MemoryCache.Default;

        public Repository(TSFContext context)
        {
            dbCtx = context;
            ctxID = ShortUID();
            dbSet = dbCtx.Set<TEntity>();
        }

        public DbContext GetContext()
        {
            return dbCtx;
        }

        private Exception LogException(Exception ex)
        {
            string message = $"Entity had {ex.Message} error.";

            var validationErrors = dbCtx.ChangeTracker.Entries<IValidatableObject>().SelectMany(e => e.Entity.Validate(null)).Where(r => r != ValidationResult.Success);
            if (validationErrors.Any())
            { 
                var msg = new StringBuilder("The DbEntityValidationException was caught while saving changes. ");
                foreach (var error in validationErrors)
                {
                    msg.Append($"Property: {error.GetType().Name}, Error: {error.ErrorMessage}").AppendLine();
                }
                message = msg.ToString();
            }
            else if (ex is DbUpdateException)
            {
                var dex = ex as DbUpdateException;
                var msg = new StringBuilder("The DbUpdateException was caught while saving changes. ");
                foreach (var result in dex.Entries)
                {
                    msg.AppendFormat($"Type: {result.Entity.GetType().Name} was part of the problem. ");
                }
                message = msg.ToString();
            }
            else if (ex is DbUpdateConcurrencyException)
            {
                var dex = ex as DbUpdateConcurrencyException;
                var msg = new StringBuilder("The DbUpdateConcurrencyException was caught while saving changes. ");
                foreach (var result in dex.Entries)
                {
                    msg.AppendFormat($"Type: {result.Entity.GetType().Name} was part of the problem. ");
                }
                message = msg.ToString();
            }

            GlobalLoggingHandler.Logging.Fatal(message, ex);
            return ex;
        }

        public void Add(TEntity entity, string editor = null)
        {
            DisableDetectChangeThen(EntityConfigAdd, () => dbSet.Add(entity));
        }

        public void Update(TEntity entity, string editor = null)
        {
            DisableDetectChangeThen(EntityConfigUpdate, () =>
            {
                dbSet.Attach(entity);
                dbCtx.Entry(entity).State = EntityState.Modified;
            });
        }

        public void Update(TEntity entity, params Expression<Func<TEntity, object>>[] @orders)
        {
            DisableDetectChangeThen(EntityConfigUpdate, () =>
            {
                dbSet.Attach(entity);
                foreach (var obj in @orders)
                {
                    dbCtx.Entry(entity).Property(obj).IsModified = true;
                }
            });
        }

        public void Delete(TEntity entity)
        {
            DisableDetectChangeThen(EntityConfigDelete, () => dbSet.Remove(entity));
        }

        public void Delete(Expression<Func<TEntity, bool>> @exp)
        {
            DisableDetectChangeThen(EntityConfigDelete, () =>
            {
                var objects = dbSet.Where(@exp).AsEnumerable();
                foreach (var obj in objects) { dbSet.Remove(obj); }
            });
        }

        public TEntity Find(Expression<Func<TEntity, bool>> @exp)
        {
            return dbSet.FirstOrDefault(@exp);
        }

        public IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> @exp)
        {
            return dbSet.Where(@exp);
        }

        public IQueryable<TEntity> Table { get => dbSet; }

        public bool SaveChanges()
        {
            try
            {
                ChangeTracker();
                var result = dbCtx?.SaveChanges();
                if (result > 0)
                    return true;
                if (result == 0)
                    GlobalLoggingHandler.Logging.Error($"[ctxID:{ctxID}][CacheID[{Cache.Get(CacheIDKey)}] Entity had {result} of objects written to the underlying database.");
                else // On result is -1
                    GlobalLoggingHandler.Logging.Error($"[ctxID:{ctxID}][CacheID[{Cache.Get(CacheIDKey)}] Entity had fail written to the database.");
                return false;
            }
            catch (DbUpdateConcurrencyException cex)
            {
                var exception = LogException(cex);
                throw exception;
            }
            catch (DbUpdateException vex)
            {
                var exception = LogException(vex);
                throw exception;
            }
            catch (Exception ex)
            {
                var exception = LogException(ex);
                throw exception;
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                ChangeTracker();
                var result = await dbCtx?.SaveChangesAsync();
                if (result > 0)
                    return true;
                if (result == 0)
                    GlobalLoggingHandler.Logging.Error($"[ctxID:{ctxID}][CacheID[{Cache.Get(CacheIDKey)}] Entity had {result} of objects written to the underlying database.");
                else // On result is -1
                    GlobalLoggingHandler.Logging.Error($"[ctxID:{ctxID}][CacheID[{Cache.Get(CacheIDKey)}] Entity had fail written to the database.");
                return false;
            }
            catch (DbUpdateConcurrencyException cex)
            {
                var exception = LogException(cex);
                throw exception;
            }
            catch (DbUpdateException vex)
            {
                var exception = LogException(vex);
                throw exception;
            }
            catch (Exception ex)
            {
                var exception = LogException(ex);
                throw exception;
            }
        }

        public void Dispose()
        {
            dbCtx?.Dispose();
        }

        public void AddRange(IList<TEntity> entities, string editor = null)
        {
            DisableDetectChangeThen(EntityConfigAdd, () => dbCtx?.Set<TEntity>().AddRange(entities));
        }

        public IQueryable<TEntity> FindByDescending<T>(Expression<Func<TEntity, bool>> @exp, Expression<Func<TEntity, T>> @order)
        {
            return dbSet.Where(@exp).OrderByDescending(@order);
        }

        private void DisableDetectChangeThen(string operationKey, Action operation)
        {
            Exception operationException = null;
            try
            {
                DetectChange(operationKey, false);
                operation();
            }
            catch (Exception e)
            {
                operationException = e;
            }
            finally
            {
                DetectChange(operationKey, true);
            }
            if (operationException != null)
            {
                throw operationException;
            }
        }

        private string ShortUID()
        {
            var uid = Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "");
            return uid;
        }

        private void DetectChange(string key, bool isDetectChange)
        {
            if (BoolTryParse(GetCacheValue(key)))
            {
                dbCtx.ChangeTracker.AutoDetectChangesEnabled = isDetectChange;
            }
        }

        private string GetCacheValue(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                // Return empty string on empty key.
                return "";
            }
            if (!Cache.Contains(key))
            {
                GlobalLoggingHandler.Logging.Info($"[ctxID:{ctxID}][CacheID[{Cache.Get(CacheIDKey)}] Cache key:`{key}` expired. Reinitializing cache.");
                CacheInitialize();
            }
            if (!Cache.Contains(key))
            {
                GlobalLoggingHandler.Logging.Error($"[ctxID:{ctxID}][CacheID[{Cache.Get(CacheIDKey)}] Reinitializing cache for key:`{key}` failed.");
                return "";
            }
            return Cache.Get(key).ToString();
        }

        private void CacheInitialize()
        {
            if (dbCtx == null)
            {
                GlobalLoggingHandler.Logging.Error($"[ctxID:{ctxID}] Can not intialize Cache for AutoDetectChangesEnabled, db context is null");
                return;
            }
            if (!Cache.Contains(CacheIDKey))
            {
                var id = ShortUID();
                Cache.Set(new CacheItem(CacheIDKey, id), new CacheItemPolicy { AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration });
                GlobalLoggingHandler.Logging.Info($"[ctxID:{ctxID}] Initialize CacheID[{Cache.Get(CacheIDKey)}]");
            }
            else
            {
                GlobalLoggingHandler.Logging.Info($"[ctxID:{ctxID}] Reinitial CacheID[{Cache.Get(CacheIDKey)}]");
            }

            var efCacheTime = 1800;
            var policy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now.AddSeconds(1800) };
            if (efCacheTime == 0)
            {
                // Default when efCacheTime = 0, AutoDetectChangesEnabled will set to true
                Cache.Remove(EntityConfigAdd);
                Cache.Remove(EntityConfigUpdate);
                Cache.Remove(EntityConfigDelete);
                Cache.Set(new CacheItem(EntityConfigCache, efCacheTime), policy);
                dbCtx.ChangeTracker.AutoDetectChangesEnabled = true;
                return;
            }
        }

        private void ChangeTracker()
        {
            var add = BoolTryParse(GetCacheValue(EntityConfigAdd));
            var update = BoolTryParse(GetCacheValue(EntityConfigUpdate));
            var delete = BoolTryParse(GetCacheValue(EntityConfigDelete));

            if (add || update || delete)
            {
                GlobalLoggingHandler.Logging.Debug($"[ctxID:{ctxID}][CacheID[{Cache.Get(CacheIDKey)}] Start Detect Changes");
                dbCtx?.ChangeTracker.DetectChanges();
                GlobalLoggingHandler.Logging.Debug($"[ctxID:{ctxID}][CacheID[{Cache.Get(CacheIDKey)}] End Detect Changes");
            }
        }

        public static bool BoolTryParse(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return false;

            try
            {
                return Convert.ToBoolean(source);
            }
            catch
            {
                return false;
            }
        }
    }
}
