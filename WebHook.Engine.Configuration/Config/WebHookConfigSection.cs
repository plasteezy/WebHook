using System.Configuration;

namespace WebHook.Engine.Configuration.Config
{
    public class WebHookConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("modules", IsRequired = true)]
        public ProviderSettingsCollection Modules
        {
            get { return (ProviderSettingsCollection)base["modules"]; }
            set { base["modules"] = value; }
        }
    }
}