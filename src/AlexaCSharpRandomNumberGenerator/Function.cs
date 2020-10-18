using System;
using System.Collections.Generic;
using Alexa.NET;
using Amazon.Lambda.Core;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AlexaCSharpRandomNumberGenerator
{
    public class Function
    {
        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            context.Logger.Log($"The input is:");
            context.Logger.Log(input == null ? "[null]" : JsonConvert.SerializeObject(input));
            // var jsonContext = JsonConvert.SerializeObject(context);
            // context.Logger.Log($"The Lambda Context is:");
            // context.Logger.Log(jsonContext);

            var session = input.Session;
            session.Attributes ??= new Dictionary<string, object>();

            switch (input.Request)
            {
                case LaunchRequest _:
                {
                    const string speech = "Welcome! Say new game to start";
                    var rp = new Reprompt("Say new game to start");
                    return ResponseBuilder.Ask(speech, rp, session);
                }

                case SessionEndedRequest _:
                    return ResponseBuilder.Tell("Goodbye!");

                case IntentRequest intentRequest:
                {
                    switch (intentRequest.Intent.Name)
                    {
                        case "RandomNumberGenerator":
                            int min = 0, max = int.MaxValue;
                            
                            var slots = intentRequest.Intent.Slots;
                            if (slots.TryGetValue("min", out var minSlot) && !string.IsNullOrEmpty(minSlot.Value))
                                min = Convert.ToInt32(minSlot.Value);
                            if (slots.TryGetValue("max", out var maxSlot) && !string.IsNullOrEmpty(maxSlot.Value))
                                max = Convert.ToInt32(maxSlot.Value);

                            return ResponseBuilder.Tell(new Random(DateTime.Now.Millisecond).Next(min, max).ToString());
                        
                        case "AMAZON.CancelIntent":
                        case "AMAZON.StopIntent":
                            return ResponseBuilder.Tell("Goodbye!");
                        case "AMAZON.HelpIntent":
                        {
                            const string msg = "What's next?";
                            return ResponseBuilder.Ask($"Here's some help. {msg}", new Reprompt(msg), session);
                        }
                        
                        default:
                        {
                            context.Logger.LogLine($"Unknown intent: " + intentRequest.Intent.Name);
                            const string msg = "Unknown command";
                            return ResponseBuilder.Ask(msg, new Reprompt(msg), session);
                        }
                    }
                }
                    
                default:
                    throw new ArgumentException(message: "RequestType is not a recognized request", paramName: nameof(input.Request.Type));
            }
        }
    }
}
