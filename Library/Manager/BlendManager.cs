using Library.Container.Blend;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Library.Manager
{
    /// <summary>
    /// Provide Multi-account blending
    /// </summary>
    public class BlendManager
    {
        internal TweetTail owner;
        private string savePath;

        private List<BlendedAccount> blendeds = new List<BlendedAccount>();
        public ReadOnlyCollection<BlendedAccount> ReadOnlyBlendedAccounts => blendeds.AsReadOnly();
        public BlendedAccount SelectedBlendedAccount {
            get {
                if (selectedBlendName == null) return null;
                return blendeds.Find((data) => { return data.Name == selectedBlendName; });
            }
        }
        private string selectedBlendName;
        public string SelectedBlendName {
            get {
                return selectedBlendName;
            }
            set {
                selectedBlendName = value;
                Save();
            }
        }
        
        public BlendManager(TweetTail owner)
        {
            this.owner = owner;
            savePath = Path.Combine(owner.SaveDir, "blends.json");
            Load();
        }

        public void RegisterBlendedAccount(BlendedAccount blendedAccount)
        {
            blendeds.Add(blendedAccount);
            Save();
        }

        public BlendedAccount GetBlendedAccount(string name)
        {
            return blendeds.Find((data) => { return data.Name == name; });
        }

        public void UnregisterBlendedAccount(BlendedAccount blendedAccount)
        {
            blendeds.RemoveAll((data) => { return data.Name == blendedAccount.Name; });
            Save();
        }

        private void Load()
        {
            if( !File.Exists(savePath) )
            {
                return;
            }

            var json = JObject.Parse( File.ReadAllText(savePath) );
            selectedBlendName = json["selectedBlendName"].ToString();

            foreach(var blendJson in json["blendeds"].ToObject<JArray>())
            {
                blendeds.Add(BlendedAccount.Load(owner, blendJson.ToObject<JObject>()));
            }
        }

        public void Save()
        {
            var json = new JObject();
            json["selectedBlendName"] = selectedBlendName;

            var blendArray = new JArray();
            
            foreach(var blended in blendeds)
            {
                blendArray.Add(blended.Save());
            }
            json["blendeds"] = blendArray;

            File.WriteAllText(savePath, json.ToString());
        }
    }
}
