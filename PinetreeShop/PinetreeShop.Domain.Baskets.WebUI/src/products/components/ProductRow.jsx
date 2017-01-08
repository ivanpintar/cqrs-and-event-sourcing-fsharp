import React from 'react';
import { Button } from 'react-bootstrap';

const ProductRow = (product, clickToAddToBasket) => (
    <tr key={product.id}>
        <td>{product.name}</td>
        <td>{product.price}</td>
        <td>
            <Button 
                className='btn btn-success' 
                onClick={clickToAddToBasket}>
                +
            </Button>
        </td>
    </tr>
)

export default ProductRow;