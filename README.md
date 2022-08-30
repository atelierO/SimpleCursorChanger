SimpleCursorChanger ![apm](https://img.shields.io/apm/l/vim-mode.svg)
===
- C#/.NET5.0로 작성된 커서 이미지 파일 간단하게 교체하는 프로그램입니다
-  Program for changing cursor image simply by C#/.NET5.0

![Preview Image](img/preview.png)

사용법 / Usage
===
- ...을 클릭해 커서 이미지를 포함한 폴더를 선택
- 자신에 환경에 맞는 커서 사이즈 선택 후 Apply 클릭   
- Click ... to select folder including cursor images
- Click apply after set cursor size to fit on your environment    

바꿀 커서 이미지의 이름은 아래의 포맷에 맞추어 작성해야 프로그램이 인식한다   
The name of cursor image for changing should be written in format below   

| 이미지<br>image |  한글명  | English Name  |  포맷<br>Format  |
|-----------------|----------|---------------|-----------------|
| ![i1](img/Arrow.png)      | 일반 선택               | Normal Select | Arrow |
| ![i2](img/Help.png)       | 도움말 선택             | Help Select  | Help |
| ![i3](img/AppStarting.png)| 백그라운드 작업         | Working in Background | AppStarting |
| ![i4](img/Wait.png)       | 사용 중                 | Busy  | Wait |
| ![i5](img/cross.png)      | 정밀도 선택             | Precision Select | Crosshair   |
| ![i6](img/beam.png)       | 텍스트 선택             | Text Select | IBeam       |
| ![i7](img/NWPen.png)      | 필기                    | Handwriting | NWPen       |
| ![i8](img/No.png)         | 사용할 수 없음          | Unavailable   | No          |
| ![i9](img/SizeNS.png)     | 수직 크기 조절          |  Vertical Resize | SizeNS      |
| ![i10](img/SizeWE.png)    | 수평 크기 조절          | Horizontal Resize   | SizeWE      |
| ![i11](img/SizeNWSE.png)  | 대각선 방향 크기 조절 1 | Diagonal Resize 1  | SizeNWSE    |
| ![i12](img/SizeNESW.png)  | 대각선 방향 크기 조절 2 | Diagonal Resize 2 | SizeNESW    |
| ![i13](img/SizeAll.png)   | 이동                   | Move  | SizeAll     |
| ![i14](img/UpArrow.png)   | 대체 선택              | Alternate Select  | UpArrow     |
| ![i15](img/Hand.png)      | 연결 선택              | Link Select  | Hand        |
| ![i16](img/Pin.png)       | 위치 선택              | Location Selet  | Pin         |
| ![i17](img/Person.png)    | 사용자 선택            | Person Select  | Person      |

예시 / Example
---
![preview_ex](img/preview_example.png)
