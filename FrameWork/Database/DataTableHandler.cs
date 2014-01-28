using System.Collections;
using System.Data;

namespace FrameWork
{
    public class DataTableHandler
    {
        private readonly ICache _cache;
        private readonly DataSet _dset;
        private readonly Hashtable _precache;
        private bool _hasRelations;

        public DataTableHandler(DataSet dataSet)
        {
            _cache = new SimpleCache();
            _precache = new Hashtable();
            _dset = dataSet;
            _hasRelations = false;
        }

        public bool HasRelations
        {
            get { return _hasRelations; }
            set { _hasRelations = false; }
        }

        public ICache Cache
        {
            get { return _cache; }
        }

        public DataSet DataSet
        {
            get { return _dset; }
        }

        public bool UsesPreCaching { get; set; }

        public void SetCacheObject(object key, DataObject obj)
        {
            _cache[key] = obj;
        }

        public DataObject GetCacheObject(object key)
        {
            return _cache[key] as DataObject;
        }

        public void SetPreCachedObject(object key, DataObject obj)
        {
            _precache[key] = obj;
        }

        public DataObject GetPreCachedObject(object key)
        {
            return _precache[key] as DataObject;
        }
    }
}