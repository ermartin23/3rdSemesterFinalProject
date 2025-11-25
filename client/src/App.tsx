import "./App.css";
import {createBrowserRouter, type RouteObject, RouterProvider} from "react-router";

const myRoutes: RouteObject[]
  = [
  {
    path: '/',
    element: <Home/>
  },
  {
    path: '/settings',
    element: <div>Hello there, this is settings route test:))))</div>
  }
]

function Home()
{
  return (
    <div>home</div>
  )
}


function App() {

  return <RouterProvider router={createBrowserRouter(myRoutes)} />
}

export default App;
