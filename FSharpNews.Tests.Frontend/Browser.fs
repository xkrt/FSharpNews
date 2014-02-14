[<AutoOpen>]
module FSharpNews.Tests.Frontend.Browser

open System
open OpenQA.Selenium
open OpenQA.Selenium.Chrome
open OpenQA.Selenium.Support.UI
open FSharpNews.Tests.Core

let private driver = new ChromeDriver(Environment.executingAssemblyDirPath())
let private jsexecutor = driver :> IWebDriver :?> IJavaScriptExecutor

let start () = ()
let close () = driver.Quit()
let go (url: string) = driver.Navigate().GoToUrl url

let private waitFor assertFn =
    let wait = WebDriverWait(driver, TimeSpan.FromSeconds(20.))
    let lastException = ref null
    try
        wait.Until(fun _ ->
            try
                assertFn(); true
            with
            | ex -> lastException := ex; false
        ) |> ignore
    with
    | :? WebDriverTimeoutException -> failwithf "Timeout exception: %O" !lastException

let element selector = fun () -> driver.FindElement(By.CssSelector selector)
let elementWithin selector (parentElementFn: unit -> IWebElement) = parentElementFn().FindElement(By.CssSelector selector)
let elementsWithin selector (parentElementFn: unit -> IWebElement) = fun () -> parentElementFn().FindElements(By.CssSelector selector) |> Seq.toList

let findElement selector (parent: IWebElement) = parent.FindElement(By.CssSelector selector)
let findElements selector (parent: IWebElement) = parent.FindElements(By.CssSelector selector) |> Seq.toList

let private displayed (elem: IWebElement) = elem.Displayed
let private text (elem: IWebElement) = elem.Text

let click (elementFn: unit -> IWebElement) = elementFn().Click()

let checkDisplayed elementFn = waitFor (fun () -> elementFn() |> displayed |> assertEqual true)
let checkNotDisplayed elementFn = waitFor (fun () -> elementFn() |> displayed |> assertEqual false)
let checkTextIs expectedText elementFn = waitFor (fun () -> elementFn() |> text |> assertEqual expectedText)    
let waitTitle text = waitFor (fun () -> driver.Title |> assertEqual text)

let scrollToBottom () =
    let script =
        """var targetHeight = $(document).height();
           while($(window).scrollTop() + $(window).height() < targetHeight)
               scrollTo(0, $(window).scrollTop() + $(window).height() + 50)"""
    jsexecutor.ExecuteScript(script) |> ignore

let (?) (element: IWebElement) attr = element.GetAttribute(attr)
