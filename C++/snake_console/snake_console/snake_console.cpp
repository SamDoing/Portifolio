#include <iostream>
#include <conio.h>
#include <Windows.h>
#include <thread>


const int width = 50, height = 20;
const int sizeX = width + 1, sizeY = height + 1;

int headX, headY, fruitX, fruitY, score, numberBody;
char map[sizeY][sizeX];
int myBodyX[100], myBodyY[100];
int hPX, hPY;

bool gameOver;


enum eDirection { STOP, LEFT, RIGHT, UP, DOWN };
eDirection dir;

void CreateMap()
{
	for (int j = 0; j < sizeY; j++)
	{
		for (int i = 0; i < sizeX; i++)
		{
			if (j == 0 || j == sizeY - 1)
				map[j][i] = '#';
			else
			{
				if (i == 0 || i == sizeX - 2)
					map[j][i] = '#';
				else
					map[j][i] = ' ';

			}
			if (i == sizeX - 1)
				map[j][i] = '\n';
		}
	}
}

void Setup()
{
	gameOver = false;
	dir = STOP;
	score = 0;
	numberBody = 0;
	auto startPos = [] { headX = width / 2; headY = height / 2; fruitX = std::rand() % width + 1; fruitY = std::rand() % height + 1; };
	startPos();
	CreateMap();
}

void Draw(bool& timed, HANDLE& std_out)
{
	if (!timed)
		return;

	//CLS
	SetConsoleCursorPosition(std_out, { 0,0 });
	for (int y = 0; y < sizeY; y++)
	{
		for (int x = 0; x < sizeX; x++)
		{
			if (fruitX == x && fruitY == y)
				std::cout << 'F';
			else if (headX == x && headY == y)
				std::cout << 'O';
			else if (numberBody > 0)
				for (int i = 0; i < numberBody; i++)
				{
					if (myBodyX[i] == x && myBodyY[i] == y)
					{
						std::cout << 'o';
						break;
					}
					else if (i == numberBody - 1)
						std::cout << map[y][x];
				}
			else
				std::cout << map[y][x];
		}
	}
	std::cout << "Score: " << score;
}

void Input()
{
	if (_kbhit())
	{
		auto cd = _getch();
		switch (cd)
		{
		case 'w':
		case 72:
			if (dir != DOWN)
				dir = UP;
			break;
		case 's':
		case 80:
			if (dir != UP)
				dir = DOWN;
			break;
		case 'd':
		case 77:
			if (dir != LEFT)
				dir = RIGHT;
			break;
		case 'a':
		case 75:
			if (dir != RIGHT)
				dir = LEFT;
			break;
		}
	}
}

void Logic(bool& timed)
{
	if (!timed)
	{
		timed = true;
		return;
	}
	timed = false;

	auto stayInsideMap = [](const char& c, const int& value) { if (c == 'x' && 0 < headX && headX < width - 1) headX += value;
	else if (c == 'y' && 0 < headY && headY < height) headY += value; };
	if (numberBody > 0) {
		int prevX = myBodyX[0], prevY = myBodyY[0];
		int prev2X, prev2Y;
		for (int i = 1; i < numberBody; i++)
		{
			prev2X = myBodyX[i];
			prev2Y = myBodyY[i];
			myBodyX[i] = prevX;
			myBodyY[i] = prevY;
			prevX = prev2X;
			prevY = prev2Y;
		}
		myBodyX[0] = headX, myBodyY[0] = headY;
	}

	switch (dir)
	{
	case LEFT:
		stayInsideMap('x', -1);
		break;
	case RIGHT:
		stayInsideMap('x', 1);
		break;
	case UP:
		stayInsideMap('y', -1);
		break;
	case DOWN:
		stayInsideMap('y', 1);
		break;
	}

	if (map[headY][headX] == '#')
		gameOver = true;
	else if (headX == fruitX && headY == fruitY)
	{
		score += 10;
		numberBody++;
		myBodyX[numberBody - 1] = hPX;
		myBodyY[numberBody - 1] = hPY;
		do {
			fruitX = (std::rand() % width) + 1;
			fruitY = (std::rand() % height) + 1;
		} while (fruitX > width - 2 || fruitY > height - 1);
	}
	else {
		for (int i = 0; i < numberBody; i++)
			if (headX == myBodyX[i] && headY == myBodyY[i])
				gameOver = true;
	}
}

//hide the console cursor
void ShowConsoleCursor(bool showFlag, HANDLE& out)
{


	CONSOLE_CURSOR_INFO	cursorInfo;

	GetConsoleCursorInfo(out, &cursorInfo);
	cursorInfo.bVisible = showFlag; // set the cursor visibility
	SetConsoleCursorInfo(out, &cursorInfo);
}


int main()
{
	Setup();
	HANDLE std_out = GetStdHandle(STD_OUTPUT_HANDLE);
	ShowConsoleCursor(false, std_out);
	bool timed = false;
	while (!gameOver)
	{

		Draw(timed, std_out);
		Input();
		Logic(timed);
		Sleep(30);
	}
	return 0;
}