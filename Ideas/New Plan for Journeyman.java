interface IViewable {
  string getView();
}

class PropertyValue {
  int value;
  
  // Посылает observer сообщение об изменении
}

class PropertyValueObserver {
  // Уведомляет подписчиков об изменении источника подписки
}

class CalculatedPropertyValue extends PropertyValue {
  boolean isObservePropertys; // Если сетится value то отключается, когда переводится в true пересчитыает value
  
  List<PropertyValue> PropertyValues;
  // и функция по которой расчитывается defolt значение
  
  // Посылает observer сообщение об изменении
  // Принимает от observer сообщения об изменении в свойствах за которыми следит
}

class Property implements IViewable {
  string name;
  int indexNumber; // Еще тип для UI
  PropertyValue defolt;
  PropertyValue current;
  
  IPropertyValueViewFormatter formatter;
}

interface IPropertyViewFormatter {
  // всякие модификаторы значений
  string format(string name, PropertyState property);
}

class PropertyState {
  final int defolt;
  final int current;
  final int min;
  final int max;
}

class RestrictedProperty extends Property {
  PropertyValue min;
  PropertyValue max;
}

// IInventory, IState, IListPr
class Character {
  List<Property> properties;
  State state; // Могут хранить некоторое поведение при renew, работает на подобие Effect патерн State
  List<Skill> skills;
  List<Item> items; // Возможно нужен класс Inventory в котором и лежит List<Item>
  Body body; // Сюда экипируются предметы
  List<Effect> effects; // EffectsColection так как можно про разному внутри организовать логику работы с колекциями эффектов
  List<IAction> actions;
  
  void doAction(int id);
  
  void addItem(Item item);
  
  Item removeItem(int id);
  
  void addEffect(Effect effect);
  
  Effect removeEffect(int id);
  
  void renew(); // update
}

class Body {
  List<BodyPart> parts; // Возможно нужен класс BodyParts в котором и лежит List<BodyPart>
  
  List<Item> equip(List<Item> items);
  
  List<Item> unequip(List<int> itemIds);
  
  void add(BodyPart part);
  
  BodyPart remove(int id);
}

class BodyPart {
  int id;
  string name;
  Item item;
}

class State {
}

class Skill {
  List<Property> properties;
  List<Target> targets; // нечто реализующее интерфейс Target: -> Character, Item, Effect
  List<Action> actions;
}

class Item {
  List<Property> properties;
  int size; // возможно достаточно поля в properties
  State state; // возможно достаточно поля в properties
  
  boolean isActive();
}

interface IEquippable {
  List<Item> equip(List<BodyPart> bodyParts);
}

class ItemEquippable extends Item implements IEquippable {
  // Тут тоже должно быть что-то типо патерна State, так как в зависимости от размера предмета по сравнению с размером персонажа
  // Он может быть двуручным/одноручным или терять возможность быть экипированным
  Map<int, BodyPart> slots;
}

class Effect {
  List<Property> properties;
}

class PermanentEffect extends Effect {
}

interface ITurnable {
  void turn();
}

class TemporaryEffect extends Effect implements ITurnable {
  int timeAction;
}

interface IAction {
  ?Target? source
  List<Target> targets; // нечто реализующее интерфейс Target: -> Character, Item, Effect
  // возможно его не нужно хранить получаем в методе
  
  void execute();
}
