import { useState } from "react";
import reactLogo from "./assets/react.svg";
import viteLogo from "/vite.svg";
import "./App.css";
import {createBrowserRouter, type RouteObject, RouterProvider} from "react-router-dom";

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

function Home() {
    return (
        <>
            <div>home!</div>
            <button>click me!
            </button>
        </>
    )
}

function App() {

  return <RouterProvider router={createBrowserRouter(myRoutes)} />
}

export default App;
