using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Windows.Input;
using MauiApp1.Models;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;

namespace MauiApp1.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    public ObservableCollection<ShoppingItem> Items { get; set; } = new();
    public string NewItemName { get; set; }

    public ICommand AddItemCommand { get; }
    public ICommand RemoveItemCommand { get; }

    string filePath => Path.Combine(FileSystem.AppDataDirectory, "shoppinglist.json");

    public MainViewModel()
    {
        AddItemCommand = new Command(AddItem);
        RemoveItemCommand = new Command<ShoppingItem>(RemoveItem);
        LoadItems();
    }

    void AddItem()
    {
        if (!string.IsNullOrWhiteSpace(NewItemName))
        {
            Items.Add(new ShoppingItem { Name = NewItemName.Trim() });
            NewItemName = string.Empty;
            OnPropertyChanged(nameof(NewItemName));
            SaveItems();
        }
    }

    void RemoveItem(ShoppingItem item)
    {
        Items.Remove(item);
        SaveItems();
    }

    void SaveItems()
    {
        var json = JsonSerializer.Serialize(Items);
        File.WriteAllText(filePath, json);
    }

    void LoadItems()
    {
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            var loaded = JsonSerializer.Deserialize<ObservableCollection<ShoppingItem>>(json);
            if (loaded is not null)
                foreach (var item in loaded)
                    Items.Add(item);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
