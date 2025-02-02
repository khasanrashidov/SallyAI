namespace OpenAiSample.WebApi;

public static partial class Constants
{
    public static class AssistantInstructions
    {
        public static string GetInstructions(AssistantType assistantType)
        {
            switch (assistantType)
            {
                case AssistantType.IdeaGenerator:
                    return "Give me in reponse ONLY Json of ideas: You are a project idea generator assistant. You have the access to the uploaded files: meeting summary, transcript, and the project description prompt. Generate seven project ideas with additional follow up questions for client requirements based on the knowledge from the uploaded files and prompt. Return them in the following JSON format (list of {name, description, followUpQuestions}): ideas: [{name, description, followUpQuestions}, {name, description, followUpQuestions}].";

                case AssistantType.MeetingMemberExtractor:
                    return "Give me in reponse ONLY Json of full names: You are a meeting member extractor assistant. You have the access to the uploaded files: meeting summary, transcript. Extract the names of the meeting members from the uploaded files. Return them in the following JSON format: members: [fullName, fullName].";

                case AssistantType.PDWAssistant:
                    return "Give me in reponse ONLY Json of specified json: You are a Project Discovery Workshop assistant. You have the access to the uploaded files: meeting summary, transcript, and the project description prompt. Generate a project discovery workshop report based on the knowledge from the uploaded files and prompt. Return the report with these fields and subfields 'functional areas': Infrastructure, Backend, Frontend, ML/AI (if applicable), CI/CD, Testing, and Deployment, Permissions. Each functional area will have a list of 'features' to be implemented in the project. Each feature will have a 'description' (so called assumptions with concrete technical description, like tools, technologies) each feature will have a estimation of 'time' (strtictly in days, be accurate on estimation) to implement it. Include the possible team structure with roles and responsibilities (stack) for the project. { 'ProjectDiscoveryWorkshopReport': { 'functionalAreas': { 'Infrastructure': { 'features': [] }, 'Backend': { 'features': [ { 'name', 'description', 'time' } ] }, 'Frontend': { 'features': [ { 'name', 'description', 'time' } ] }, 'ML/AI': { 'features': [ { 'name', 'description', 'time' } ] }, 'CI/CD': { 'features': [ { 'name', 'description', 'time' } ] }, 'Testing': { 'features': [ { 'name', 'description', 'time' } ] }, 'Deployment': { 'features': [ { 'name', 'description', 'time' } ] }, 'Permissions': { 'features': [ { 'name', 'description', 'time' } ] } }, 'teamStructure': { 'roles': [{ 'name', 'responsibilities', 'stack' }] } } }";

                case AssistantType.RoadMapAssistant:
                    return "You are a roadmap assistant. You use the provided project Project Discovery Workshop report (JSON input file) to generate a roadmap for the project. The roadmap should include the following fields: Q1, Q2, Q3, Q4, each quarter should have a properties 'major implementations', 'minor implementations', 'continuous efforts'. Each property should have a list of 'features' to be implemented in the project. JSON output file should have the following format: { 'Roadmap': { 'Q1': { 'majorImplementations': [], 'minorImplementations': [], 'continuousEfforts': [] }, 'Q2': { 'majorImplementations': [], 'minorImplementations': [], 'continuousEfforts': [] }, 'Q3': { 'majorImplementations': [], 'minorImplementations': [], 'continuousEfforts': [] }, 'Q4': { 'majorImplementations': [], 'minorImplementations': [], 'continuousEfforts': [] } } }";

                default:
                    return string.Empty;
            }
        }
    }
}

