namespace ProgressService.Services.Templates
{
    public static class StepSmsTemplateBuilder
    {
        public static string Build(int stepId, string? link)
        {
            return stepId switch
            {
                1 => $"The project has been created. You can access it by opening this link: {link}", 
                2 => "We ordered materials for your roof.", 
                3 => "The delivery is scheduled. Roofers will show up soon.", 
                4 => "We have started installing your roof.", 
                5 => "Warranty & Documents are ready for you", 
                6 => "Pay Us",
                _ => "test"
            };
        }
    }
}
