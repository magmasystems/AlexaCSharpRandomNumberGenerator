using System;
using System.Collections.Generic;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Xunit;
using Amazon.Lambda.TestUtilities;

namespace AlexaCSharpRandomNumberGenerator.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestLambdaFunction()
        {
	        var function = new Function();
            var context = new TestLambdaContext();
            var request = new SkillRequest
            {
	            Request = new IntentRequest
	            {
		            Type = nameof(IntentRequest),
		            Timestamp = DateTime.Now,
		            Locale = "en-US",
		            RequestId = "amzn1.echo-api.request.75628577-8998-46bb-a24e-14e629aaa637",
		            Intent = new Intent
		            {
			            Name = "RandomNumberGenerator",
			            ConfirmationStatus = "NONE",
			            Slots = new Dictionary<string, Slot>
			            {
				            {"min", new Slot {Name = "min", SlotValue = new SlotValue {Value = "100"}, Value = "100", ConfirmationStatus = "NONE", Source = "USER"}},
				            {"max", new Slot {Name = "max", SlotValue = new SlotValue {Value = "200"}, Value = "200", ConfirmationStatus = "NONE", Source = "USER"}}
			            }
		            }
	            },
	            Session = new Session()
            };

            var response = function.FunctionHandler(request, context);

            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.NotNull(response.Response.OutputSpeech);
            
            var speech = response.Response.OutputSpeech as PlainTextOutputSpeech;
            Assert.NotNull(speech);
            Assert.NotNull(speech.Text);
            var num = Convert.ToInt32(speech.Text);
            Assert.True(num >= 100 && num <= 200);
        }
    }
}
