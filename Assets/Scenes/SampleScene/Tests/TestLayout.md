**LayoutGroup * * — это абстрактный базовый класс в Unity uGUI для создания layout-контроллеров. Он реализует интерфейсы `ILayoutElement`, `ILayoutController` и управляет размерами и позициями дочерних RectTransform элементов.

## Поля (Fields)

| Поле | Тип | Описание |
|------|-----|----------|
| `padding` | `RectOffset` | Внутренние отступы по краям группы |
| `m_Padding` | `RectOffset` | Приватное поле для padding (сериализуется) |
| `spacing` | `float` | Расстояние между дочерними элементами |
| `m_Spacing` | `float` | Приватное поле для spacing |
| `childAlignment` | `TextAnchor` | Выравнивание дочерних элементов |
| `m_ChildAlignment` | `TextAnchor` | Приватное поле для выравнивания |
| `childControlWidth` | `bool` | Контролировать ли ширину дочерних |
| `childControlHeight` | `bool` | Контролировать ли высоту дочерних |
| `childForceExpandWidth` | `bool` | Растягивать ли дочерние по ширине |
| `childForceExpandHeight` | `bool` | Растягивать ли дочерние по высоте |
| `childControlSize` | `Vector2` | Размер, к которому принудительно приводятся дети |
| `minWidth` | `float` | Минимальная ширина группы |
| `minHeight` | `float` | Минимальная высота группы |
| `preferredWidth` | `float` | Предпочтительная ширина группы |
| `preferredHeight` | `float` | Предпочтительная высота группы |
| `flexibleWidth` | `float` | Гибкая ширина группы |
| `flexibleHeight` | `float` | Гибкая высота группы |
| `layoutPriority` | `int` | Приоритет layout'а |

## Свойства (Properties)

| Свойство | Тип | Описание |
|----------|-----|----------|
| `rectChildren` | `IList<RectTransform>` | Список всех RectTransform детей |
| `transform` | `Transform` | Трансформ группы |
| `rectTransform` | `RectTransform` | RectTransform группы |
| `minWidth`/`minHeight` | `float` | (из ILayoutElement) |
| `preferredWidth`/`preferredHeight` | `float` | (из ILayoutElement) |
| `flexibleWidth`/`flexibleHeight` | `float` | (из ILayoutElement) |
| `layoutPriority` | `int` | (из ILayoutElement) |

## Методы (Methods)

### Абстрактные (обязательны для переопределения):
```csharp
public abstract void CalculateLayoutInputHorizontal();
public abstract void CalculateLayoutInputVertical();
public abstract void SetLayoutHorizontal();
public abstract void SetLayoutVertical();
```

### Виртуальные (можно переопределить):
```csharp
protected virtual void OnEnable();
protected virtual void OnDisable();
protected virtual void OnDidApplyAnimationProperties();
public virtual void SetProperty<T>(ref T currentValue, T newValue);
```

### Основные публичные методы:
```csharp
public void CalculateLayoutInputForAxis(int axis);
public void SetLayoutInputForAxis(int axis);
public void SetLayoutInputForGroup(bool horizontal);
public bool IsChildControlWidthControlled(RectTransform child);
public bool IsChildControlHeightControlled(RectTransform child);
```

### Приватные/защищенные методы:
```csharp
protected RectTransform[] GetChildList();
protected IList<RectTransform> rectChildren { get; }
```

## Наследники LayoutGroup

```
LayoutGroup(абстрактный)
├── HorizontalLayoutGroup
│   └── HorizontalOrVerticalLayoutGroup
│       ├── HorizontalLayoutGroup
│       └── VerticalLayoutGroup
├── VerticalLayoutGroup
├── GridLayoutGroup
├── ContentSizeFitter (частично использует LayoutGroup логику)
└── Custom Layout Groups (ваши наследники)
```

## Детали реализации ключевых наследников

### HorizontalLayoutGroup
```
Дополнительные поля:
-reverseArrangement(bool)
Методы: GetChildHorizontalSize(), GetChildVerticalSize()
```

### VerticalLayoutGroup  
```
Аналогично HorizontalLayoutGroup
```

### GridLayoutGroup
```
Дополнительные поля:
-startCorner(Corner)
- startAxis(GridLayoutGroup.Axis)
- cellSize(Vector2)
- spacing(Vector2)
- constraint(GridLayoutGroup.Constraint)
- constraintCount(int)
Методы: CellToLocalPoint(), LocalToCell()
```

## Интерфейсы, реализуемые LayoutGroup

```
ILayoutElement:
├── minWidth / minHeight
├── preferredWidth / preferredHeight
├── flexibleWidth / flexibleHeight
└── layoutPriority

ILayoutController:
├── childControlsWidth/Height
└── childForceExpandWidth/Height
```

## Цикл жизни LayoutGroup

```
1. OnEnable() → CalculateLayoutInputHorizontal/Vertical()
2. LayoutRebuilder.MarkLayoutForRebuild()
3. CalculateLayoutInputForAxis(axis)
4. SetLayoutInputForAxis(axis) 
5. SetLayoutHorizontal/Vertical()
6. LayoutRebuilder.ForceRebuildLayoutImmediate()
```

Этот полный API позволяет создавать любые кастомные layout'ы, полностью контролируя расчет размеров и позиционирование дочерних элементов.
