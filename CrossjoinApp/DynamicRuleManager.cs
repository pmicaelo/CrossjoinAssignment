using System.ComponentModel.DataAnnotations;

public class Rule
{
    public string ClassName { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public Func<object, bool> Condition { get; set; } = _ => false;
    public bool IsRequired { get; set; }
}

public class DynamicRuleManager
{
    private readonly List<Rule> rules = new();

    public void SetRequired<T>(string className, string fieldName, Func<T, bool> condition, bool isRequired)
    {
        rules.Add(new Rule
        {
            ClassName = className,
            FieldName = fieldName,
            Condition = obj => condition((T)obj),
            IsRequired = isRequired,
        });
    }

    public void Validate(object instance)
    {

        string className = instance.GetType().Name;

        foreach (Rule rule in rules)
        {
            if (rule.ClassName == className && rule.IsRequired){

                if (rule.Condition(instance))
                {
                    var prop = instance.GetType().GetProperty(rule.FieldName);
                    if (prop != null)
                    {
                        var value = prop.GetValue(instance);
                        if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                        {
                            throw new ValidationException($"Field '{rule.FieldName}' is required for {className}.");
                        }
                    }
                }
            }
        }
    }
}