using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AddressBookServer.Models;

namespace AddressBookServer.Controllers
{
    //Helper class to sort query string by size desc, and remove null string to not search by it.
    public class SearchComposer : IDictionary<ContactProperties, string>
    {
        Dictionary<ContactProperties, string> terms = new Dictionary<ContactProperties, string>();
        Dictionary<ContactProperties, string> termsTemp = default;

        public SearchComposer(string firstName, string lastName, string address, string phone)
        {
            termsTemp = new Dictionary<ContactProperties, string>()
            {
                {ContactProperties.FirstName, firstName },
                {ContactProperties.LastName, lastName },
                {ContactProperties.Address, address },
                {ContactProperties.Phone, phone }
            };
        }

        public void Prepare()
        {
            List<string> order = new List<string>();
            foreach (var p in termsTemp.Keys)
            {
                if (!(termsTemp[p] == null))
                {
                    order.Add(termsTemp[p]);
                }
            }

            terms.Clear();
            if (order.Count == 0)
            {
                return;
            }

            order.Sort((text1, text2) => text2.Length - text1.Length);

            foreach (var o in order)
            {
                foreach (var key in termsTemp.Keys)
                {
                    if (termsTemp[key] == o)
                    {
                        terms.Add(key, o);
                    }
                }
            }
            order.Clear();
            termsTemp.Clear();
        }

        #region IDictionary implementation

        public string this[ContactProperties key] { get => ((IDictionary<ContactProperties, string>)terms)[key]; set => ((IDictionary<ContactProperties, string>)terms)[key] = value; }

        public ICollection<ContactProperties> Keys => ((IDictionary<ContactProperties, string>)terms).Keys;

        public ICollection<string> Values => ((IDictionary<ContactProperties, string>)terms).Values;

        public int Count => ((IDictionary<ContactProperties, string>)terms).Count;

        public bool IsReadOnly => ((IDictionary<ContactProperties, string>)terms).IsReadOnly;

        public void Add(ContactProperties key, string value)
        {
            ((IDictionary<ContactProperties, string>)terms).Add(key, value);
        }

        public void Add(KeyValuePair<ContactProperties, string> item)
        {
            ((IDictionary<ContactProperties, string>)terms).Add(item);
        }

        public void Clear()
        {
            ((IDictionary<ContactProperties, string>)terms).Clear();
        }

        public bool Contains(KeyValuePair<ContactProperties, string> item)
        {
            return ((IDictionary<ContactProperties, string>)terms).Contains(item);
        }

        public bool ContainsKey(ContactProperties key)
        {
            return ((IDictionary<ContactProperties, string>)terms).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<ContactProperties, string>[] array, int arrayIndex)
        {
            ((IDictionary<ContactProperties, string>)terms).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<ContactProperties, string>> GetEnumerator()
        {
            return ((IDictionary<ContactProperties, string>)terms).GetEnumerator();
        }

        public bool Remove(ContactProperties key)
        {
            return ((IDictionary<ContactProperties, string>)terms).Remove(key);
        }

        public bool Remove(KeyValuePair<ContactProperties, string> item)
        {
            return ((IDictionary<ContactProperties, string>)terms).Remove(item);
        }

        public bool TryGetValue(ContactProperties key, [MaybeNullWhen(false)] out string value)
        {
            return ((IDictionary<ContactProperties, string>)terms).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<ContactProperties, string>)terms).GetEnumerator();
        }
        #endregion IDictionary
    }
}
