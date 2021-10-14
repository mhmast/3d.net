#pragma once
namespace _3DNet::Rendering::D3D11::Interop {

	public ref class Device : public System::IDisposable
	{

		!Device() {
			DisposeInternal(false);
		}
	private:
		void DisposeInternal(bool disposing);
	public:

		Device() {}
		~Device();
		void BeginScene();
		void Initialize();

		~Device()
		{
			System::GC::SuppressFinalize(this);
			DisposeInternal(true);
		}

	};
}