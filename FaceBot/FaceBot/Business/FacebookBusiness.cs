﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using FaceBot.Repository;

namespace FaceBot.Business
{
    class FacebookBusiness
    {
        private FacebookRepository repository;

        public FacebookBusiness()
        {
            repository = new FacebookRepository();
        }

        public List<String> getListUsers()
        {
            List<String> list = new List<string>();

            list.Add("Mariana Macena");
            list.Add("Samantha Cortez");
            list.Add("Silas Pascoal Dos Santos");
            list.Add("Bruna Rodrigues");
            list.Add("Victor Osés");
            list.Add("Kevin Meeira");
            list.Add("Juliana Costa Melo");
            
            return list;
        }

        public void LoginAndRedirectPostFacebook() {
            List<String> friends;
            bool flag = true;

            FirefoxOptions options = new FirefoxOptions();
            options.AddArgument("--start-maximized");

            using (var driver = new FirefoxDriver(options)) {
                try
                {
                    driver.Manage().Window.Maximize();
                    
                    //Realizando Login
                    driver.Navigate().GoToUrl(ConfigurationManager.AppSettings["facebookLogin"]);
                    driver.FindElementByCssSelector("input#email").SendKeys(ConfigurationManager.AppSettings["login"]);
                    driver.FindElementByCssSelector("input#pass").SendKeys(ConfigurationManager.AppSettings["senha"]);
                    driver.FindElementByCssSelector("button#loginbutton").Click();

                    //Pressiona botão ESC
                    Actions action = new Actions(driver);
                    action.SendKeys(Keys.Escape).Build().Perform();
                    
                    driver.Navigate().GoToUrl(ConfigurationManager.AppSettings["facebookPost"]);

                    friends = getListUsers();

                    while (true)
                    {
                        //Comentando usando o layout mobile
                        foreach (var friend in friends)
                        {
                            //Comenta nome dos amigos no facebook
                            Thread.Sleep(2000);
                            flag = CommentUsersInPost(driver, friend);
                            if (!flag)
                                break;
                        }

                        driver.Navigate().GoToUrl(ConfigurationManager.AppSettings["facebookPostNewLayout"]);

                        //Comentando usando o layout padrão
                        foreach (var friend in friends)
                        {
                            //Comenta nome dos amigos no facebook
                            Thread.Sleep(2000);
                            flag = CommentUsersInPostByNewLayout(driver, friend);
                            if (!flag)
                                break;
                        }
                    }

                }
                catch (Exception ex)
                {
                    new FacebookRepository().Log(null, ex.Message);
                }
            }
        }

        public bool CommentUsersInPost(FirefoxDriver driver, string friend)
        {
            bool isExist = true;

            while (isExist)
            {
                try
                {
                    driver.FindElementByCssSelector("textarea#composerInput").Click();
                    isExist = false;
                }
                catch (Exception)
                {
                    isExist = true;
                }
            }

            Thread.Sleep(1100);
            driver.FindElements(By.CssSelector("[class*='cp dy dz']")).First().Click();
            Thread.Sleep(1100);
            driver.FindElement(By.CssSelector("[class*='bp bq br bs']")).Click();
            Thread.Sleep(1100);
            driver.FindElement(By.CssSelector("[class*='bn bo bp']")).SendKeys(friend);
            Thread.Sleep(1600);
            driver.FindElement(By.CssSelector("[class*='bb bc bd bq br']")).Click();
            Thread.Sleep(1100);
            var checkBoxConfirmFriend = driver.FindElement(By.CssSelector("[class*='n']"));
            checkBoxConfirmFriend.FindElements(By.XPath("//input[@type='checkbox']")).First().Click();
            Thread.Sleep(1100);
            driver.FindElement(By.CssSelector("[class*='bb bc bd cb br']")).Click();
            Thread.Sleep(1100);
            driver.FindElement(By.CssSelector("[class*='be bf bg']")).Click();
            Thread.Sleep(1100);
            driver.FindElement(By.CssSelector("[class*='be bf bg']")).SendKeys(".");
            Thread.Sleep(1100);
            driver.FindElement(By.CssSelector("[class*='bm bd bv bw bx']")).Click();
            
            //Validando bloqueio temporário
            try
            {
                driver.FindElement(By.CssSelector("[class*='bl bm']")).Click();
                new FacebookRepository().Log(friend, "Você foi bloqueado pois está usando os recursos do facebook rápido demais");
                return false;
            }
            catch (Exception ex)
            {
                new FacebookRepository().Log(friend, ex.Message);
            }

            try
            {
                driver.FindElementByCssSelector("textarea#composerInput").Click();
                new FacebookRepository().Insert(friend);
                return true;
            }
            catch (Exception ex)
            {
                new FacebookRepository().Log(friend, ex.Message);
                return false;
            }
        }

        public bool CommentUsersInPostByNewLayout(FirefoxDriver driver, string friend) {


            return false;
        }
    }
}
