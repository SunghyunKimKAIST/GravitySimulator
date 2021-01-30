import win32file
import win32pipe
import pywintypes
import torch
from models import RPATRecursive

model = RPATRecursive(
    num_particles=3,
    dimension=2,
    n_hidden_features=256,
    dropout=0.1,
    alpha=0.2,
    n_heads=8
)
model = model.cuda()
model.eval()
model.load_state_dict(torch.load('train_model/horse_3.pkl'))


def send_calc_ended(pipe):
    win32file.WriteFile(pipe, 'calc ended'.encode())
    win32file.FlushFileBuffers(pipe)


def send_particles(pipe, particles):
    s = ''
    for x in particles.flatten():
        s += f'{x}\n'
    win32file.WriteFile(pipe, s[:-1].encode())
    win32file.FlushFileBuffers(pipe)


def server(pipe_name):
    while True:
        pipe = win32pipe.CreateNamedPipe(
            rf'\\.\pipe\{pipe_name}',
            win32pipe.PIPE_ACCESS_DUPLEX,
            win32pipe.PIPE_TYPE_MESSAGE | win32pipe.PIPE_READMODE_MESSAGE | win32pipe.PIPE_WAIT,
            1, 65536, 65536, 0, None)

        try:
            print('waiting for unity')
            win32pipe.ConnectNamedPipe(pipe, None)

            resp = win32file.ReadFile(pipe, 1024)
            assert resp[0] == 0
            resp_split = resp[1].splitlines()
            duration = float(resp_split[0])
            print(duration)
            particles = torch.tensor([float(x) for x in resp_split[1:]]).view(3, 4)
            print(particles)

            emptyStates = [torch.empty(size=(3, 4)) for _ in range(int(16 * duration))]
            emptyStates.insert(0, particles)
            inputStates = torch.stack(emptyStates).cuda()

            outputStates = model(inputStates)

            print('calc ended')
            send_calc_ended(pipe)
            for state in outputStates:
                send_particles(pipe, state)

            print('end')

        except pywintypes.error as e:
            print(e)

        finally:
            win32file.CloseHandle(pipe)


if __name__ == '__main__':
    server('3')
