using Entities;
using SV.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class KeyService
    {
        #region singleton
        public static KeyService Instance
        {
            get
            {
                if (instance == null) instance = new KeyService();
                return instance;
            }
        }
        private static KeyService instance { get; set; }
        #endregion
        public List<Key> keydata(string name)
        {
            try
            {
                using (var context = new SVIContext())
                {
                    var result = context.Keys.Where(x => x.Type == name).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Testemonial> Testemonails()
        {
            try
            {
                using (var context = new SVIContext())
                {
                    var result = context.Testemonials.OrderByDescending(x=>x.Id).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Key SeasonImg(string name)
        {
            try
            {
                using (var context = new SVIContext())
                {
                    var result = context.Keys.Where(x => x.Type == name).FirstOrDefault();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
