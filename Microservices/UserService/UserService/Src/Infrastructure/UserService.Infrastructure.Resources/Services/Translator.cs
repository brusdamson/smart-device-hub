using System.Globalization;
using System.Resources;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Infrastructure.Resources.ProjectResources;

namespace UserService.Infrastructure.Resources.Services
{
    public class Translator : ITranslator
    {

        private readonly ResourceManager resourceMessages;
        private readonly ResourceManager resourceGeneral;

        public string this[string name] => resourceGeneral.GetString(name, CultureInfo.CurrentCulture) ?? name;

        public Translator()
        {
            resourceMessages = new ResourceManager(typeof(ResourceMessages).FullName, typeof(ResourceMessages).Assembly);
            resourceGeneral = new ResourceManager(typeof(ResourceGeneral).FullName, typeof(ResourceGeneral).Assembly);
        }
        public string GetString(string name)
        {
            return resourceMessages.GetString(name, CultureInfo.CurrentCulture) ?? name;
        }

        public string GetString(TranslatorMessageDto input)
        {
            var value = resourceMessages.GetString(input.Text, CultureInfo.CurrentCulture) ?? input.Text.Replace("_", " ");
            return string.Format(value, input.Args);
        }
    }
}
