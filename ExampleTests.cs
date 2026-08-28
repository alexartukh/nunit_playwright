using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace MyProject.Tests {
    [TestFixture]
    public class ExampleTests : PageTest {
        [Test]
        public async Task HasTitle() {
            await Page.GotoAsync("https://playwright.dev");

            await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));
        }

        [Test]
        public async Task GetStartedLink() {
            await Page.GotoAsync("https://playwright.dev");

            await Page.GetByRole(AriaRole.Link, new() { Name = "Get started" }).ClickAsync();

            await Expect(Page.Locator("h1")).ToContainTextAsync("Installation");
        }
    }
}