using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace MyProject.Tests {
    [TestFixture]
    public class PlaceOrderTests : PageTest {
        public override BrowserNewContextOptions ContextOptions() {
            return new BrowserNewContextOptions() {
                IgnoreHTTPSErrors = true
            };
        }

        [Test]
        public async Task RegisterUser_Failed() {
            await Page.GotoAsync("https://std131001.test/Mein-Konto", new PageGotoOptions {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            await Page.Locator("a:has-text('Konto anlegen')").ClickAsync();
            await Expect(Page.Locator("body")).ToContainTextAsync("Daten bearbeiten", new() { Timeout = 10000 });

            await Page.Locator("#kd_vorname").FillAsync("ALEX");
            await Page.Locator("#kd_nachname").FillAsync("ARTUKH");

            // SS
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshots/registration-page-before.png" });

            await Expect(Page.Locator("body")).Not.ToContainTextAsync("Bitte füllen Sie alle markierten Pflichtfelder aus!", new() { Timeout = 10000 });

            await Page.Locator("input[name='button_submit']").ClickAsync(new() { Timeout = 10000 });

            await Expect(Page.Locator("body")).ToContainTextAsync("Bitte füllen Sie alle markierten Pflichtfelder aus!", new() { Timeout = 10000 });

            // SS
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshots/registration-page-after.png" });
        }

        [Test]
        public async Task PlaceOrder() {
            // таймаут на весь тест (в C# NUnit задаётся через атрибут или CancelAfter)
            // альтернатива — см. примечание ниже

            // лог для отчёта (аналог testInfo.attach с текстом)
            await AttachText("setTimeout", "Global timeout = 60000");

            // my account
            await Page.GotoAsync("https://std131001.test/Mein-Konto", new PageGotoOptions {
                WaitUntil = WaitUntilState.NetworkIdle
            });
            await Page.Locator("input[name=\"kd_mail\"]").FillAsync("alex@example.com");
            await Page.Locator("input[name=\"kd_pwd\"]").FillAsync("123");
            await Page.Locator("input.loginBtn").ClickAsync();
            await Expect(Page.Locator("body")).ToContainTextAsync("Bestellungen einsehen", new() { Timeout = 10000 });

            // home page
            await Page.GotoAsync("https://std131001.test/");
            await Page.Locator("a[alt=\"p1\"]").ClickAsync();
            //await Page.WaitForTimeoutAsync(2000);

            // PDP
            await Expect(Page.Locator("body")).ToContainTextAsync("In den Warenkorb", new() { Timeout = 10000 });
            await Page.Locator("input[value=\"In den Warenkorb\"]").ClickAsync();
            //await Page.WaitForTimeoutAsync(2000);
            await Page.Locator("a[alt=\"zum Warenkorb\"]").WaitForAsync(new() { State = WaitForSelectorState.Visible });
            //await Page.WaitForTimeoutAsync(2000);
            await Page.Locator("a[alt=\"zum Warenkorb\"]").ClickAsync(new() { Timeout = 10000 });

            // WK
            await Page.Locator("input[name=\"button_submit\"]").WaitForAsync(new() { State = WaitForSelectorState.Visible });
            await Page.Locator("input[name=\"dhl_agree_checkbox\"]").CheckAsync();
            await Page.Locator("input[name=\"button_submit\"]").ClickAsync(new() { Timeout = 10000 });
            await Page.WaitForTimeoutAsync(4000);

            // step 3
            await Expect(Page.Locator("body")).ToContainTextAsync("Adresse und Lieferung", new() { Timeout = 10000 });
            await Page.Locator("input[name=\"best_vorname\"]").WaitForAsync(new() { State = WaitForSelectorState.Visible });
            await Page.Locator("input[name=\"button_submit\"]").ClickAsync(new() { Timeout = 10000 });
            await Page.WaitForTimeoutAsync(4000);

            // step 4
            await Expect(Page.Locator("body")).ToContainTextAsync("Zahlungsart und Versand", new() { Timeout = 10000 });
            await Page.Locator("input[name=\"bestagb\"]").WaitForAsync(new() { State = WaitForSelectorState.Visible });
            await Page.Locator("input[name=\"bestagb\"]").CheckAsync();
            await Page.Locator("input[name=\"button_submit\"]").ClickAsync(new() { Timeout = 10000 });
            await Page.WaitForTimeoutAsync(4000);

            // step 5
            await Expect(Page.Locator("body")).ToContainTextAsync(
                "Ein letzter Check - bitte kontrollieren Sie Ihre Angaben vor dem Absenden nochmals auf ihre Richtigkeit.",
                new() { Timeout = 10000 });
            await Page.Locator("input[value=\"zahlungspflichtig bestellen\"]").WaitForAsync(new() { State = WaitForSelectorState.Visible });
            await Page.Locator("input[value=\"zahlungspflichtig bestellen\"]").ClickAsync(new() { Timeout = 10000 });
            await Page.WaitForTimeoutAsync(10000);

            // thank you page
            await Expect(Page.Locator("body")).ToContainTextAsync("Vielen Dank für Ihre Bestellung", new() { Timeout = 10000 });

            var headingText = await Page.Locator("h5[role=\"heading\"]").InnerTextAsync();
            var match = Regex.Match(headingText, @"\d+$");
            if (match.Success) {
                var orderNumber = match.Value;
                await AttachText("orderNumber", "Order Number = " + orderNumber);
            }
        }

        // Вспомогательный метод — аналог testInfo.attach() с текстовым содержимым.
        // В NUnit нет прямого эквивалента attach с телом текста, поэтому пишем во временный файл.
        private static Task AttachText(string name, string body) {
            var dir = Path.Combine(TestContext.CurrentContext.WorkDirectory, "attachments");
            Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, $"{name}_{Guid.NewGuid():N}.txt");
            File.WriteAllText(path, body);
            TestContext.AddTestAttachment(path, name);
            return Task.CompletedTask;
        }
    }
}