# Моделирование эволюции биологических популяций
[English](https://github.com/IliaTrofimov/PopulationModels/tree/master#population-dynamics-modeling)

Симуляция и визуализация развития биологических популяций пары "хищник-жертва" по модели А. Д. Базыкина [1]. За основу взята информация из [статьи](https://moluch.ru/archive/106/25234/) [2].

## Описание модели 
На данный момент программа реализует следующую систему дифференциальных уравнений для описания динамики "хищник-жертва":

$$
\begin{cases}
\frac{dx}{dt} = Ax - B \frac{xy}{1 + px} - Ex^2 \\
\frac{dy}{dt} = -Cy + D \frac{xy}{1 + px} - Mx^2 \\
\end{cases}
$$

также есть усложнённая систем, где параметры $B$ и $D$ зависят от времени $t$:

$$
\begin{cases}
\frac{dx}{dt} = Ax - B(t) \frac{xy}{1 + px} - Ex^2 \\
\frac{dy}{dt} = -Cy + D(t) \frac{xy}{1 + px} - Mx^2 \\
B(t) = B_0 \sin(\frac{\pi t}{period}) \\
D(t) = D_0 \sin(\frac{\pi t}{period}) \\
\end{cases}
$$

[Методы Рунге-Кутты](https://en.wikipedia.org/wiki/Runge%E2%80%93Kutta_methods) (2-го и 4-го порядка) использованы для численного решения системы.

## Реализация

Приложение базируется на .NET 8.0 и использует [AvaloniaUI](https://avaloniaui.net/) для кросс-платформенного пользовательского интерфейса (вместо WPF) и [ScottPlot](https://scottplot.net/) для отображения графиков. 
Также я самостоятельно реализовал версию метода Рунге-Кутты на C# для оптимизации использования памяти и ускорения вычислений: 
для хранения результата и всех промежуточных данных используетя один непрерывный массив (см. `ArrayPool`), чтобы снизить число аллокаций и событий сборки мусора; 
по возможности все опреации с векторами выполняются на месте, без создания копий.

## Использование
Для ОС Windows (протестировано на 10) достаточно загрузить последний релиз и запустить .exe файл в нём. Для Mac OS на ARM возможно запустить приложение из Visual Studio или Rider, пока что не удалось собрать проект в цельную программу для Mac.

## Интерфейс
![image](https://github.com/user-attachments/assets/169a20e0-49c8-4c7c-b884-da22dddace6f)

![image](https://github.com/user-attachments/assets/ac6568bc-06dd-4e98-917f-3b0dd945f04e)

![image](https://github.com/user-attachments/assets/04798c35-47be-4b24-bdb5-caf0a45e3e26)

## Источники
- [1] - Базыкин А. Д. Нелинейная динамика взаимодействующих популяций. Москва-Ижевск: Институт компьютерных технологий, 2003. — 368 с.
- [2] - Колпак, Е. П. О моделях А. Д. Базыкина «хищник — жертва» / Е. П. Колпак, Е. В. Горыня, Е. А. Селицкая. — Текст : непосредственный // Молодой ученый. — 2016. — № 2 (106). — С. 12-20. — URL: https://moluch.ru/archive/106/25234/ (дата обращения: 05.12.2024).


---


# Population dynamics modeling
Simulation and visualization of evolution of biological populations of prey and predator spicies. This application implements differential equation system invented by A. D. Bazykin [1]. Inspired by [this](https://moluch.ru/archive/106/25234/) parer [2].

## Model description
Currently program implements this ODE system for prey-predators dynamics:

$$
\begin{cases}
\frac{dx}{dt} = Ax - B \frac{xy}{1 + px} - Ex^2 \\
\frac{dy}{dt} = -Cy + D \frac{xy}{1 + px} - Mx^2 \\
\end{cases}
$$

and more sophisticated system where parameters $B$ and $D$ depends on time value:

$$
\begin{cases}
\frac{dx}{dt} = Ax - B(t) \frac{xy}{1 + px} - Ex^2 \\
\frac{dy}{dt} = -Cy + D(t) \frac{xy}{1 + px} - Mx^2 \\
B(t) = B_0 \sin(\frac{\pi t}{period}) \\
D(t) = D_0 \sin(\frac{\pi t}{period}) \\
\end{cases}
$$

[Runge-Kutta methods](https://en.wikipedia.org/wiki/Runge%E2%80%93Kutta_methods) (2nd and 4th order) are used to solve ODE systems.

## Implementation

Application is built with .NET 8.0 and uses [AvaloniaUI](https://avaloniaui.net/) for cross-platform user interface (instead of WPF) and [ScottPlot](https://scottplot.net/) for plotting. 
I also had implemented my own version of Runge-Kutta method in C# to optimize memory usage (I am storing solution in one continuous pulled `Array<double>` to reduce allocations and GC events).

## Usage
For Windows (tested on  Windows 10) you should download latest release and run .exe file from it. To run on Mac OS with ARM you should clone repo and run UI project in Visual Studio or in Rider (currently I don't know how to pack it as Mac application).

## Sources
- [1] - Базыкин А. Д. Нелинейная динамика взаимодействующих популяций. Москва-Ижевск: Институт компьютерных технологий, 2003. — 368 с.
- [2] - Колпак, Е. П. О моделях А. Д. Базыкина «хищник — жертва» / Е. П. Колпак, Е. В. Горыня, Е. А. Селицкая. — Текст : непосредственный // Молодой ученый. — 2016. — № 2 (106). — С. 12-20. — URL: https://moluch.ru/archive/106/25234/ (дата обращения: 05.12.2024).
