using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ImageManipulator.Infrastructure.Tab;
using Moq;
using TabItem = ImageManipulator.Application.Common.Models.TabItem;

namespace Infrastructure.UnitTests;

[ExcludeFromCodeCoverage]
public class TabServiceTests
{
    private Mock<IServiceProvider> _serviceProvider;
    
    private Mock<IQueryDispatcher> _queryDispatcher;
    private Mock<ICommandDispatcher> _commandDispatcher;
    
    private TabService _tabService;
    
    [SetUp]
    public void Setup()
    {
        _serviceProvider = new Mock<IServiceProvider>();
        _queryDispatcher = new Mock<IQueryDispatcher>();
        _commandDispatcher = new Mock<ICommandDispatcher>();
        
        _serviceProvider.Setup(x => x.GetService(typeof(TabControlViewModel)))
            .Returns(new TabControlViewModel(_queryDispatcher.Object, _commandDispatcher.Object));
        
        _tabService = new TabService(_serviceProvider.Object);
    }

    [Test]
    public void AddEmptyReturnsExpectedTabItem()
    {
        TabItem expectedTabItem = new TabItem("custom tab",
            new TabControlViewModel(_queryDispatcher.Object, _commandDispatcher.Object));
        
        TabItem tabItem = _tabService.AddEmpty(expectedTabItem);
        
        Assert.That(tabItem, Is.SameAs(expectedTabItem));
        Assert.True(tabItem.Name == "custom tab");
    }

    [Test]
    public void GetTabReturnsExpectedTab()
    {
        TabItem expectedTabItem = new TabItem("Tab 1",
            new TabControlViewModel(_queryDispatcher.Object, _commandDispatcher.Object));

        TabItem? tabItem = _tabService.GetTab("Tab 1");
        
        Assert.That(tabItem.Name, Is.EqualTo(expectedTabItem.Name));
    }
    
    [Test]
    public void GetTabReturnsNull()
    {
        TabItem? tabItem = _tabService.GetTab("unknown");
        
        Assert.IsNull(tabItem);
    }

    [Test]
    public void AddExistingTabGeneratesNewName()
    {
        TabItem? tabItem = _tabService.AddExistingTab("Tab 1",
            new TabControlViewModel(_queryDispatcher.Object, _commandDispatcher.Object));
        
        Assert.True("Tab 1-new" == tabItem.Name);
    }
    
    [Test]
    public void AddExistingTabDoesNotGenerateNewName()
    {
        TabItem? tabItem = _tabService.AddExistingTab("Tab 2",
            new TabControlViewModel(_queryDispatcher.Object, _commandDispatcher.Object));
        
        Assert.True("Tab 2" == tabItem.Name);
    }

    [Test]
    public void RemoveExistingTabShouldRemoveTab()
    {
        TabItem? tabItem = _tabService.AddEmpty(new TabItem("Tab 2", new TabControlViewModel(_queryDispatcher.Object, _commandDispatcher.Object)));
        Assert.AreEqual("Tab 2", tabItem.Name);

        Assert.DoesNotThrow(() => _tabService.RemoveTab("Tab 2"));
        
        TabItem? nullTab = _tabService.GetTab("Tab 2");
        Assert.IsNull(nullTab);
    }

    [Test]
    public void ReplaceShouldReplaceTabWithNewTab()
    {
        TabItem item = new TabItem("Replaced", new TabControlViewModel(_queryDispatcher.Object, _commandDispatcher.Object)
        {
            Path = "Replaced"
        });

        TabItem? replaced = _tabService.Replace("Tab 1", item);
        
        Assert.That(replaced, Is.SameAs(item));
        Assert.AreEqual("Replaced", replaced.Name);
    }

    [Test]
    public void ReplaceThrowsNullReferenceException()
    {
        Assert.Throws<NullReferenceException>(() => _tabService.Replace("nonExisting",
            new TabItem(new TabControlViewModel(_queryDispatcher.Object, _commandDispatcher.Object))));
    }

    [Test]
    public void DuplicateShouldCreateDuplicatedTab()
    {
        TabItem? duplicated = _tabService.Duplicate("Tab 1");
        TabItem? firstTab = _tabService.GetTab("Tab 1");
        
        Assert.AreEqual("Tab 1-new", duplicated.Name);
        Assert.NotNull(firstTab);
    }

    [Test]
    public void GetTabsReturnsTabList()
    {
        ObservableCollection<TabItem> expected = new ObservableCollection<TabItem>(new List<TabItem>()
        {
            _tabService.GetTab("Tab 1")
        });
        
        ObservableCollection<TabItem> tabs = _tabService.GetTabItems();
        
        Assert.That(tabs, Is.EquivalentTo(expected));
    }

    [Test]
    public void SetTabShouldWorkAsExpected()
    {
        ObservableCollection<TabItem> expected = new ObservableCollection<TabItem>(new List<TabItem>()
        {
            new("unknown", new TabControlViewModel(_queryDispatcher.Object, _commandDispatcher.Object)
            {
                Path = "unknown"
            }),
            new("Tab 2", new TabControlViewModel(_queryDispatcher.Object, _commandDispatcher.Object)
            {
                Path = "Tab 2"
            })
        });
        
        _tabService.SetTabs(expected);
        var actual = _tabService.GetTabItems();
        
        Assert.That(actual, Is.EquivalentTo(expected));
    }
}