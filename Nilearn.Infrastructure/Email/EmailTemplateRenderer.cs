using Nilearn.Application.Common.Interfaces;

namespace Nilearn.Infrastructure.Email;

public class EmailTemplateRenderer : IEmailTemplateRenderer
{
    public string Render(string templatePath, Dictionary<string, string> values)
    {
        var fullPath = Path.Combine(
         AppContext.BaseDirectory,
         templatePath);

        var template = File.ReadAllText(fullPath);

        foreach (var value in values)
        {
            template = template.Replace($"{{{{{value.Key}}}}}", value.Value);
        }

        return template;
    }
}
