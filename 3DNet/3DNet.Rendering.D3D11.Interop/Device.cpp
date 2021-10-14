#include "Device.h"
#include <d3d11.h>

using namespace _3DNet::Rendering::D3D11::Interop;

ID3D11Device* _device;
ID3D11DeviceContext* _context;

void _3DNet::Rendering::D3D11::Interop::Device::DisposeInternal(bool disposing)
{
	_context->Release();
	delete _context;
	_context = nullptr;
	_device->Release();
	delete _device;
	_device = nullptr;
}

void Device::BeginScene() {
		_device.con
}

void _3DNet::Rendering::D3D11::Interop::Device::Initialize()
{
	throw gcnew System::NotImplementedException();
}

void _3DNet::Rendering::D3D11::Interop::Device::Dispose()
{
	System::GC::SuppressFinalize(this);
	DisposeInternal(true);
}
